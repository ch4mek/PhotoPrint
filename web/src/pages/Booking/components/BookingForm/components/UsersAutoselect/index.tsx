import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import isEqual from 'lodash/isEqual';
import React, { useMemo } from 'react';

import { useBookingContext } from '@/context/Booking';
import { getEmployeeOptions } from '@/helpers/getEmployeeOptions';

const getKey = (item: { id: number }) => item.id;

const areOptionsEqual = (
	option: { id: number },
	autoselectValue: { id: number }
) => (option as { id: number }).id === (autoselectValue as { id: number }).id;

const AutoselectMemoized = React.memo(Autoselect, isEqual) as typeof Autoselect;

export function BookingUsersAutoselect() {
	const {
		employee,
		initialEmployee,
		setEmployee,
		bookingUsersData: data,
		isFetchingBookingUsers: isFetching,
	} = useBookingContext();

	const theme = useTheme();

	const employeeOptions = useMemo(
		() => getEmployeeOptions(data, isFetching),
		[data, isFetching]
	);

	const label = employee
		? employee.positionDescription
		: initialEmployee.positionDescription;

	const value = employee && employee.name ? employee : initialEmployee;

	return (
		<AutoselectMemoized
			label={label}
			value={value}
			valueField="name"
			options={employeeOptions}
			onChange={setEmployee}
			valueToClean={initialEmployee}
			labelAndErrorBgColor={theme.palette['background-white']}
			getKey={getKey}
			checkIfOptionEqualToValue={areOptionsEqual}
		/>
	);
}
