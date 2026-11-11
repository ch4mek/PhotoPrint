import { useTheme } from '@emotion/react';
import { RadioButton } from '@mms/mms-ui-library/components/RadioButton';
import { OnChangeArgs } from '@mms/mms-ui-library/components/RadioButton/types';
import { useTranslation } from 'react-i18next';

import { useBookingContext } from '@/context/Booking';
import { BookingModes } from '@/types/booking';

import { bookingModeControleFields } from './constants';
import { RadioButtonsWrapper } from './styles';

export function BookingModeControls() {
	const { guest, employee } = bookingModeControleFields;
	const { setSelectMode, isGuestMode } = useBookingContext();
	const theme = useTheme();
	const { t } = useTranslation('booking');

	const handleSetSelectMode = (args: OnChangeArgs) => {
		setSelectMode(args.value as BookingModes);
	};

	return (
		<RadioButtonsWrapper>
			<RadioButton
				onChange={handleSetSelectMode}
				checked={!isGuestMode}
				forAttribute={employee.attributeFor}
				label={t(employee.labelKey)}
				name={employee.name}
				value={BookingModes.Employees}
				color={theme.palette['text-gray']}
			/>
			<RadioButton
				onChange={handleSetSelectMode}
				checked={isGuestMode}
				forAttribute={guest.attributeFor}
				label={t(guest.labelKey)}
				name={guest.name}
				value={BookingModes.Guest}
				color={theme.palette['text-gray']}
			/>
		</RadioButtonsWrapper>
	);
}
