import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { DataSource } from 'typeorm';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ProductModule } from 'src/product/product.module';
import { Product } from 'src/product/entities/product';
import { ExchangeRateModule } from 'src/exchange-rate/exchange-rate.module';
import { ExchangeRate } from 'src/exchange-rate/entities/exchange-rate';
import { HttpModule } from '@nestjs/axios';

@Module({
  controllers: [AppController],
  providers: [AppService],
  imports: [
    HttpModule,
    ProductModule,
    ExchangeRateModule,
    ConfigModule.forRoot(),
    TypeOrmModule.forRoot({
      type: 'mysql',
      host: process.env.DB_HOST,
      port: parseInt(process.env.DB_PORT || '3306', 10),
      username: process.env.DB_USERNAME,
      password: process.env.DB_PASSWORD,
      database: process.env.DB_NAME,
      entities: [Product, ExchangeRate],
      //synchronize: true,
      //logging: ['query', 'error'],
    }),
  ],
})
export class AppModule {
  constructor(private dataSource: DataSource) { }
}
