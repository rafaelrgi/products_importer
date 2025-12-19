import { Controller, Get } from '@nestjs/common';
import { ExchangeRateService } from './exchange-rate.service';

@Controller('exchange-rates')
export class ExchangeRateController {
  //
  constructor(private readonly exchangeRateService: ExchangeRateService) { }

  //TODO: remove, test only
  @Get('')
  get() {
    return this.exchangeRateService.getTodayFiveRates();
  }
}
