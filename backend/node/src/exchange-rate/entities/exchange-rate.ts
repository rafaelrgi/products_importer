import { Entity, Column, PrimaryGeneratedColumn, Index } from 'typeorm';

@Entity()
export class ExchangeRate {
  @PrimaryGeneratedColumn()
  Id: number;

  @Column({ type: 'date' })
  Date: Date;

  @Index()
  @Column({ type: 'date' })
  CreatedAt: Date;

  @Column({ type: 'char', length: 3 })
  Abbreviation: string;

  @Column({ type: 'decimal', precision: 14, scale: 12 })
  Rate: number;
}
