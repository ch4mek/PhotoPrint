import { MouseEvent, KeyboardEvent, Dispatch } from 'react';

import { CreateCalendarTimeParams } from '@/queries/settings/types';
import { CalendaryCountry } from '@/types/configuration';
import { TooltipContent } from '@/types/hooks';
import {
	CalendarMonthTime,
	WorkingHoursCalendarRow,
	WorkingHoursPerMonth,
} from '@/types/settings';

export interface CalendarGridColumnWidths {
	countryName: number;
	month: number;
	total: number;
}

export interface CalendarOperationsContextValues {
	onCountryRowChange: (
		rowIdx: number,
		newParts: Partial<WorkingHoursCalendarRow>
	) => void;
	createCountry: (
		countryName: string,
		onSuccess?: (newCountry: CalendaryCountry) => void,
		onError?: VoidFunction
	) => void;
	updateCountry: (
		countryId: number,
		newName: string,
		onSuccess?: (newCountry: CalendaryCountry) => void,
		onError?: VoidFunction
	) => void;
	deleteCountry: (
		countryId: number,
		onSuccess?: VoidFunction,
		onError?: VoidFunction
	) => void;
	onMonthTimeChange: (rowIdx: number, monthId: number, newTime: number) => void;
	setIsMonthTimeLoading: (
		rowIdx: number,
		monthId: number,
		isLoading: boolean
	) => void;
	createMonthTime: (
		params: CreateCalendarTimeParams,
		onSuccess?: (newTime: WorkingHoursPerMonth) => void,
		onError?: VoidFunction
	) => void;
	updateMonthTime: (
		entityId: number,
		newTime: number,
		onSuccess?: (newTime: WorkingHoursPerMonth) => void,
		onError?: VoidFunction
	) => void;
	setNewMonthTime: (
		rowIdx: number,
		monthId: number,
		newTime: CalendarMonthTime
	) => void;
	setCellHasError: (rowIdx: number, monthId: number, hasError: boolean) => void;
	onAddNewRow: (
		rowIdx: number,
		row: WorkingHoursCalendarRow,
		onLoadingEnd?: VoidFunction
	) => void;
	year: number;
	handleCellClick: (event: MouseEvent<HTMLInputElement>) => void;
	handleCellKeyDown: (event: KeyboardEvent<HTMLInputElement>) => void;
	handleInputKeyDown: (event: KeyboardEvent<HTMLInputElement>) => void;
	setTooltipContent: Dispatch<React.SetStateAction<TooltipContent>>;
}
