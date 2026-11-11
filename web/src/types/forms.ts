import { AvailableMonths } from './dates';

export interface FormUserAutoselectOption {
	id: number;
	fullName: string;
}

export interface FormAutoselectOption<IdT = number> {
	id: IdT;
	name: string;
}

export type FormAutoselectOptionWithCustomValue<IdT = number> = Omit<
	FormAutoselectOption<IdT>,
	'id'
> & {
	id?: IdT;
};

export type MonthYearOption = `${AvailableMonths}, ${number}`;
