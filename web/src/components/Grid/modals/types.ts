export interface FromToProps<T> {
	from?: T | null;
	to?: T | null;
}

export interface DateRangeFilter {
	includeNull: boolean;
	fromDate: Date | null;
	toDate: Date | null;
}
