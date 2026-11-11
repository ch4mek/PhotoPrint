export function isMonthPeriod(periodArr: Date[]): boolean;
export function isMonthPeriod(periodLength: number): boolean;
export function isMonthPeriod(period: Date[] | number) {
	return typeof period === 'number' ? period > 7 : period.length > 7;
}
