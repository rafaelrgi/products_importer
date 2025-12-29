import {
  Controller,
  Post,
  UseInterceptors,
  UploadedFile,
  Get,
  BadRequestException,
  HttpStatus,
  HttpCode,
  InternalServerErrorException,
  Query,
  NotFoundException,
} from '@nestjs/common';
import { FileInterceptor } from '@nestjs/platform-express';
import { ProductService } from './product.service';
import { ProductCsvService } from './product-csv.service';
import { Product } from './entities/product';
import { ExchangeRateService } from 'src/exchange-rate/exchange-rate.service';
import * as queryParamsDto from './dto/query-params.dto';
import { IPaginatedDto } from 'src/core/dto/paginated.dto';

const PER_PAGE = 10;
const PER_PAGE_MAX = 10;

@Controller('products')
export class ProductController {
  //
  constructor(
    private readonly productService: ProductService,
    private readonly productCsvService: ProductCsvService,
    private readonly exchangeRateService: ExchangeRateService,
  ) { }

  @Get('all')
  getAllProducts(): Promise<Product[]> {
    return this.productService.findAll();
  }

  @Get()
  async getProducts(
    @Query() query: queryParamsDto.IQueryParams,
  ): Promise<IPaginatedDto<Product>> {
    //no params? bad idea!
    if (!query || Object.keys(query).length === 0)
      query = {
        page: 1,
        perPage: PER_PAGE,
        sort: '',
        order: '',
        name: '',
        priceMin: null,
        priceMax: null,
        expirationMin: null,
        expirationMax: null,
      };

    //paginate
    query.page = Math.max(query.page, 1);
    query.perPage = Math.min(query.perPage, PER_PAGE_MAX);

    //sort
    if (query.sort) {
      if (['name', 'price', 'expiration'].indexOf(query.sort) < 0) {
        throw new BadRequestException();
      }
      query.order = (query.order || '').toUpperCase();
      if (query.order !== 'DESC') {
        query.order = 'ASC';
      }
    }

    //filter
    if (query.expirationMin) {
      if (
        typeof query.expirationMin !== 'string' ||
        !/^\d{4}-\d{2}-\d{2}$/.test(query.expirationMin)
      )
        throw new BadRequestException();
    }
    if (query.expirationMax) {
      if (
        typeof query.expirationMax !== 'string' ||
        !/^\d{4}-\d{2}-\d{2}$/.test(query.expirationMax)
      )
        throw new BadRequestException();
    }

    const result = await this.productService.paginated(query);
    if (result.meta.pageCount < result.meta.page) {
      throw new NotFoundException('Page does not exist');
    }

    return result;
  }

  @Post('upload')
  @HttpCode(HttpStatus.CREATED)
  @UseInterceptors(FileInterceptor('file'))
  async uploadFile(@UploadedFile() file: Express.Multer.File) {
    if (!file) {
      throw new BadRequestException('No valid file uploaded');
    }

    try {
      const today = new Date();
      const rates = await this.exchangeRateService.getTodayFiveRates();

      const message = await this.productCsvService.processCsv(
        file,
        today,
        rates,
      );
      return message;
    } catch (error) {
      return new InternalServerErrorException(error);
    }
  }
}
