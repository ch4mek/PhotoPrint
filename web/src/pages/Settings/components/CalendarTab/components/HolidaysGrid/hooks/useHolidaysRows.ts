import { useMemo } from 'react';

import { HolidayGridRow } from '../types';
import { getCalendarExclusionsTitle } from '../utils';

import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { formatDateString } from '@/helpers/dateToISOString';
import { GetCalendarExclusionsItem } from '@/queries/calendar/types';
import { CalendaryCountry } from '@/types/configuration';

export function useHolidaysRows(
	data: GetCalendarExclusionsItem[] | undefined,
	countries: CalendaryCountry[] | undefined,
	handleEditClick: (
		holiday: GetCalendarExclusionsItem | undefined
	) => () => void
): Array<HolidayGridRow> {
	const { role } = useAuth();

	const rows = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		const resRows = data.map(
			({ id, name, dateType, countryId, date, newDate }) => ({
				name: name || '',
				dateType: getCalendarExclusionsTitle(dateType),
				countryId: countries?.find((item) => item.id === countryId)?.name ?? '',
				date: formatDateString(date, ''),
				newDate: formatDateString(newDate, ''),
				handleEditClick: isChiefRole(role)
					? () =>
							handleEditClick({
								id,
								name: name || '',
								dateType,
								countryId,
								date,
								newDate,
							})
					: undefined,
			})
		);

		return resRows;
	}, [data]);

	return rows;
}
