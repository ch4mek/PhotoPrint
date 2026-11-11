import {
	SalaryAction,
	SalaryFilterSetter,
	SalaryRow,
	SalaryTableFilters,
	SalaryTableFiltersKeys,
} from '../SalaryTable/types';

import { FromToProps } from '@/components/Grid';
import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { UserSalary } from '@/types/teamboard';

export const salaryFiltersReducer = (
	state: SalaryTableFilters,
	{ type, payload }: SalaryAction
): SalaryTableFilters => {
	switch (type) {
		case SalaryTableFiltersKeys.date: {
			return { ...state, date: payload as FromToProps<Date> };
		}

		case SalaryTableFiltersKeys.salaryUsd: {
			return { ...state, salaryUsd: payload as FromToProps<string> };
		}

		case SalaryTableFiltersKeys.set: {
			return payload as SalaryTableFilters;
		}

		default: {
			return state;
		}
	}
};

export const generateSetFilters = (
	dispatch: React.Dispatch<SalaryAction>
): SalaryFilterSetter => ({
	date: (newDate: FromToProps<Date>) =>
		dispatch({ type: SalaryTableFiltersKeys.date, payload: newDate }),
	salaryUsd: (newSalary: FromToProps<string>) =>
		dispatch({
			type: SalaryTableFiltersKeys.salaryUsd,
			payload: newSalary,
		}),
	set: (newFilters: SalaryTableFilters) =>
		dispatch({ type: SalaryTableFiltersKeys.set, payload: newFilters }),
});

export const mapSalaryResponseToRows = (
	items: Array<UserSalary>,
	onEditClick: (itemId?: string | number) => () => void
): Array<SalaryRow> =>
	items.map(({ id, salaryFrom, salaryUsd, isCurrent }) => ({
		id,
		isCurrent,
		salaryFrom: dayjs(salaryFrom).format(DATE_FORMAT),
		salaryUsd: `${salaryUsd}$`,
		handleEditClick: onEditClick,
	}));

export const getLoaderRows = (rowsCount: number) => {
	const loaderRows: Array<SalaryRow> = [];

	for (let i = 0; i < rowsCount; i += 1) {
		loaderRows.push({
			id: i,
			showSkeleton: true,
			salaryUsd: '',
			salaryFrom: '',
		});
	}

	return loaderRows;
};
