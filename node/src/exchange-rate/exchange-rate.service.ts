import { Injectable } from '@nestjs/common';
import { HttpService } from '@nestjs/axios';
import { ExchangeRate } from './entities/exchange-rate';
import { InjectRepository } from '@nestjs/typeorm';
import { DataSource, Repository } from 'typeorm';
import { firstValueFrom } from 'rxjs/internal/firstValueFrom';
import { IExchangeRatesDto } from './dto/exchange-rates.dto';
import { AxiosResponse } from 'axios';

const API_URL: string =
  'https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.json';
const API_URL_FALLBACK: string =
  'https://latest.currency-api.pages.dev/v1/currencies/usd.json';

@Injectable()
export class ExchangeRateService {
  constructor(
    @InjectRepository(ExchangeRate)
    private exchangeRateRepository: Repository<ExchangeRate>,
    private dataSource: DataSource,
  ) { }

  findAll(): Promise<ExchangeRate[]> {
    return this.exchangeRateRepository.find();
  }

  async getTodayFiveRates(): Promise<Promise<ExchangeRate[]> | null> {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    //first we try the db
    let rates = await this._getTodayFiveRatesDb(today);
    if (rates && rates.length) {
      return rates;
    }

    //now we try the api
    rates = await this._getTodayFiveRatesApi(today, API_URL);
    if (rates && rates.length) {
      return rates;
    }

    //maybe the fallback api
    rates = await this._getTodayFiveRatesApi(today, API_URL_FALLBACK);
    if (rates && rates.length) {
      return rates;
    }

    return null;
  }

  private async _getTodayFiveRatesDb(
    today: Date,
  ): Promise<ExchangeRate[] | null> {
    return this.exchangeRateRepository.find({ where: { CreatedAt: today } });
  }

  private async _getTodayFiveRatesApi(
    today: Date,
    url: string,
  ): Promise<Promise<ExchangeRate[]> | null> {
    try {
      const httpService = new HttpService();
      const response: AxiosResponse = await firstValueFrom(
        httpService.get(url),
      );
      const ratesDto: IExchangeRatesDto = response.data as IExchangeRatesDto;
      const rates: ExchangeRate[] = this._ratesFromDto(ratesDto, today);
      await this._saveToDb(rates);
      return rates;
    } catch (error) {
      console.log(error);
      return null;
    }
  }

  private async _saveToDb(rows: ExchangeRate[]): Promise<boolean> {
    try {
      //batch insert for optimization, we may be in a huge importing operation...
      await this.dataSource
        .createQueryBuilder()
        .insert()
        .into(ExchangeRate)
        .values(rows)
        .execute();
    } catch (error) {
      console.log(error);
      return false;
    }
    return true;
  }

  private _ratesFromDto(
    ratesDto: IExchangeRatesDto,
    today: Date,
  ): ExchangeRate[] {
    const date = new Date(ratesDto.date);
    const rates: ExchangeRate[] = [
      {
        Id: 0,
        Date: date,
        CreatedAt: today,
        Abbreviation: 'BRL',
        Rate: ratesDto.usd.brl,
      },
      {
        Id: 0,
        Date: date,
        CreatedAt: today,
        Abbreviation: 'CAD',
        Rate: ratesDto.usd.cad,
      },
      {
        Id: 0,
        Date: date,
        CreatedAt: today,
        Abbreviation: 'EUR',
        Rate: ratesDto.usd.eur,
      },
      {
        Id: 0,
        Date: date,
        CreatedAt: today,
        Abbreviation: 'GBP',
        Rate: ratesDto.usd.gbp,
      },
      {
        Id: 0,
        Date: date,
        CreatedAt: today,
        Abbreviation: 'MXN',
        Rate: ratesDto.usd.mxn,
      },
    ];

    return rates;
  }
}
