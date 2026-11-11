export interface TemporaryGetDialogTextComponentParams {
	userId: number | string;
	isBlock: boolean;
	firstName: string;
	lastName: string;
	isForGuest?: boolean;
	guestName?: string | null;
}

export interface MonthData {
	period: string;
	totalTimeInMinutes: number;
}
export interface MonthlyStat {
	name: {
		date: number;
		weekDay: string;
	};
	hours: number;
	minutes: number;
}
export interface YearDataStack {
	totalTimeInMinutes: number;
	period: string;
}
export interface YearStat {
	name: string;
	hours: number;
	minutes: number;
}

export interface Data {
	timePerPeriod: { period: string; totalTimeInMinutes: number }[];
}
export interface Period {
	year: number;
	month?: string;
	quarter?: string;
}

export interface WithId {
	id: string | number;
}

export type Booleanify<T> = {
	[K in keyof T]: boolean;
};

export type SetFilters<T> = {
	[K in keyof T]: (payload: T[K]) => void;
} & {
	set: (payload: T) => void;
	reset: () => void;
};

export interface WithDisabled {
	disabled?: boolean;
}

export type HeaderStyles = {
	background?: string;
	[key: string]: string | number | undefined;
};

export type InferSpecifiedKeysByValue<T, V> = {
	[K in keyof T]: T[K] extends V ? K : never;
}[keyof T];
