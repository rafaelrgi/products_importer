import { Entity, Column, PrimaryGeneratedColumn, Index } from 'typeorm';

/*
Storing CurrencyConversions as pre-calculated columns for perfromance reasons
Calculated columns would be slow for large datasets
We meet the current requirements, any further changes may require refactorings
*/
@Entity()
export class Product {
  @PrimaryGeneratedColumn()
  Id: number;

  @Index()
  @Column({ type: 'varchar', length: 255 })
  Name: string;

  @Index()
  @Column({ type: 'decimal', precision: 10, scale: 2 })
  Price: number;

  @Index()
  @Column({ type: 'date' })
  Expiration: Date;

  @Index()
  @Column({ type: 'date' })
  CreatedAt: Date;

  // Brazilian Reais
  @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
  BRL: number | null;

  // Euro
  @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
  EUR: number | null;

  // Canadian dollar
  @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
  CAD: number | null;

  // Mexican Pesos
  @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
  MXN: number | null;

  // Libras
  @Column({ type: 'decimal', precision: 10, scale: 2, nullable: true })
  GBP: number | null;
}
