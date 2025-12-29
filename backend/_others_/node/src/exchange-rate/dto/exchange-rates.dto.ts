export interface IUsdRatesDto {
  brl: number; // Brazilian Reais
  eur: number; // Euro
  cad: number; // Canadian dollar
  mxn: number; // Mexican Pesos
  gbp: number; // Libras
}

export interface IExchangeRatesDto {
  date: string;
  usd: IUsdRatesDto;
}
