import { Module } from '@nestjs/common';
import { MulterModule } from '@nestjs/platform-express';
import { ProductController } from './product.controller';
import { Product } from './entities/product';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ProductService } from './product.service';
import { ProductCsvService } from './product-csv.service';
import { ExchangeRateService } from 'src/exchange-rate/exchange-rate.service';
import { ExchangeRate } from 'src/exchange-rate/entities/exchange-rate';

@Module({
  providers: [ProductService, ProductCsvService, ExchangeRateService],
  exports: [ProductService],
  imports: [
    Product,
    TypeOrmModule.forFeature([Product]),
    TypeOrmModule.forFeature([ExchangeRate]),
    MulterModule.register({
      dest: './uploads',
    }),
  ],
  controllers: [ProductController],
})
export class ProductModule { }
