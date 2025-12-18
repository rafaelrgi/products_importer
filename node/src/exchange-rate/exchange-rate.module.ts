import { Module } from '@nestjs/common';
import { ExchangeRateController } from './exchange-rate.controller';
import { ExchangeRate } from './entities/exchange-rate';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExchangeRateService } from './exchange-rate.service';

@Module({
  providers: [ExchangeRateService],
  exports: [ExchangeRateService],
  imports: [ExchangeRate, TypeOrmModule.forFeature([ExchangeRate])],
  controllers: [ExchangeRateController],
})
export class ExchangeRateModule { }
