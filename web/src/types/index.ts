import { Roles } from '@/constants/roles';

export enum TimePeriod {
	Year = 'Year',
	Month = 'Month',
	Quarter = 'Quarter',
}

export enum RangeTimePeriod {
	Month = 1,
	Quarter = 3,
}

export type RolesType = (typeof Roles)[keyof typeof Roles];

export enum TrackedTimeType {
	Fact = 'fact',
	Billable = 'billable',
}
