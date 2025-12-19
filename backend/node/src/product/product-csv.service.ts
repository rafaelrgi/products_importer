import { BadRequestException, Injectable } from '@nestjs/common';
import { DataSource } from 'typeorm';
import * as csv from '@fast-csv/parse';
import { ProductService } from './product.service';
import { Product } from './entities/product';
import { ICsvProductDto } from './dto/csv-product.dto';
import { ISplittedDateDto } from 'src/core/dto/splitted-date';
import { ExchangeRate } from 'src/exchange-rate/entities/exchange-rate';
import { IUsdRatesDto } from 'src/exchange-rate/dto/exchange-rates.dto';

@Injectable()
export class ProductCsvService {
  //
  //we insert rows in chunks in order to eficiently handle big files
  private _chunkSize: number = 256;
  private _chunkIndex: number;
  private _chunk: Array<Product> = new Array<Product>(this._chunkSize);

  private _skipedLines: number;
  private _today: Date;

  constructor(
    private dataSource: DataSource,
    private readonly productService: ProductService,
  ) { }

  //we receive the date here so it is the same of the exchange-rates date (just in case we need it)
  processCsv(
    file: Express.Multer.File,
    date: Date,
    exchangeRates: ExchangeRate[] | null,
  ): Promise<string> {
    const rates: IUsdRatesDto | null = exchangeRates
      ? this._ratesToDto(exchangeRates)
      : null;

    this._skipedLines = 0;
    this._chunkIndex = -1;
    //remove hours, so we avoid any troubles if some operations cross the midnight (all the products in the same date)
    this._today = date;
    this._today.setHours(0, 0, 0, 0);

    return new Promise((resolve, reject) => {
      const parser = csv
        .parseFile(file.path, { headers: true, delimiter: ';' })
        .on('error', (error) => {
          throw new BadRequestException(error);
          reject(error);
        })
        .on('data', (row: ICsvProductDto) => {
          //wait for the processing to complete before read more data
          parser.pause();
          this._processRow(row, rates)
            .then(() => parser.resume())
            .catch((error) => {
              console.error(error);
              parser.resume();
            });
        })
        .on('end', (rowCount: number) => {
          //in case the last records didn't fullfill the buffer
          void this._flushChunkToDb();
          const message = `Parsed ${rowCount} rows :: ${this._skipedLines} rejected rows`;
          console.log(message);
          resolve(message);
        });
    });
  }

  private async _processRow(row: ICsvProductDto, rates: IUsdRatesDto | null) {
    try {
      if (!this._validateRow(row)) {
        throw Error('Invalid row');
      }

      const splittedDate = this._splitDate(row);
      if (!splittedDate) {
        throw Error('Invalid row');
      }

      const price = parseFloat(row.price.replace(/[^0-9+\-.]/g, ''));
      if (Number.isNaN(price)) {
        throw Error('Invalid row');
      }

      const prices: IUsdRatesDto = this._calcPrices(price, rates);

      this._chunk[this._chunkIndex + 1] = {
        Id: 0,
        Name: row.name,
        Expiration: new Date(
          splittedDate.year,
          splittedDate.month - 1,
          splittedDate.day,
        ),
        CreatedAt: this._today,
        Price: price,
        BRL: prices.brl || null,
        CAD: prices.cad || null,
        EUR: prices.eur || null,
        GBP: prices.gbp || null,
        MXN: prices.mxn || null,
      };
      this._chunkIndex++;
    } catch {
      console.log(`Invalid row: ${JSON.stringify(row)}`);
      this._skipedLines++;
      return;
    }

    //is the buffer full?
    if (this._chunkIndex < this._chunkSize - 1) {
      return;
    }
    await this._flushChunkToDb();
  }

  private _ratesToDto(exchangeRates: ExchangeRate[]): IUsdRatesDto {
    const rates: IUsdRatesDto = {
      brl: exchangeRates.find((x) => x.Abbreviation == 'BRL')?.Rate || 0,
      eur: exchangeRates.find((x) => x.Abbreviation == 'EUR')?.Rate || 0,
      cad: exchangeRates.find((x) => x.Abbreviation == 'CAD')?.Rate || 0,
      mxn: exchangeRates.find((x) => x.Abbreviation == 'MXN')?.Rate || 0,
      gbp: exchangeRates.find((x) => x.Abbreviation == 'GBP')?.Rate || 0,
    };
    return rates;
  }

  private _calcPrices(price: number, rates: IUsdRatesDto | null): IUsdRatesDto {
    const prices: IUsdRatesDto = {
      brl: rates ? price * rates.brl : 0,
      eur: rates ? price * rates.eur : 0,
      cad: rates ? price * rates.cad : 0,
      mxn: rates ? price * rates.mxn : 0,
      gbp: rates ? price * rates.gbp : 0,
    };
    return prices;
  }

  private _validateRow(row: ICsvProductDto): boolean {
    //missing any column?
    if (!row.name || !row.price || !row.expiration) {
      return false;
    }
    row.name = row.name.trim();
    row.price = row.price.trim();
    row.expiration = row.expiration.trim();
    if (!row.name.length || !row.price.length || !row.expiration.length) {
      return false;
    }

    return true;
  }

  private _splitDate(row: ICsvProductDto): ISplittedDateDto | null {
    //get the date?
    const dateParts = row.expiration.split('/');
    if (dateParts.length != 3) {
      return null;
    }

    const year = parseInt(dateParts[2]);
    const month = parseInt(dateParts[0]);
    const day = parseInt(dateParts[1]);

    if (Number.isNaN(day) || Number.isNaN(month) || Number.isNaN(year)) {
      return null;
    }

    return {
      year: year,
      month: month,
      day: day,
    };
  }

  private async _flushChunkToDb() {
    //nothing to flush
    if (this._chunkIndex < 0) {
      return;
    }

    //is the buffer full?
    if (this._chunkIndex === this._chunkSize - 1) {
      await this._insertRows(this._chunk);
    }
    //buffer isn't full, take just the used portion
    else {
      //this copying is done just for the last insert, so no big worries
      await this._insertRows(this._chunk.slice(0, this._chunkIndex + 1));
    }

    //reset the buffer
    this._chunkIndex = -1;
  }

  private async _insertRows(rows: Array<Product>) {
    try {
      await this.dataSource
        .createQueryBuilder()
        .insert()
        .into(Product)
        .values(rows)
        .execute();
    } catch (error) {
      console.log(`*** #${this._chunkIndex}`);
      console.log(error);
    }
  }
}
