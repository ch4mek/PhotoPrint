import { useTheme } from '@emotion/react';
import { ChangeEvent, useEffect, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { ValidationError } from 'yup';

import { useBookingContext } from '@/context/Booking';
import { textType1Schema } from '@/forms/schemas/types/textType1';

import { BookingGuestInput } from './styles';

export function BookingGuestNameField() {
	const { guestName, setGuestName, guestNameError, setGuestNameError, error } =
		useBookingContext();
	const theme = useTheme();
	const { t } = useTranslation('booking');

	const nameLabel = t('form.guestNameLabel');

	const validateGuestField = async (value: string) => {
		try {
			await textType1Schema(nameLabel).validate(value);
			setGuestNameError(null);
		} catch (validationError) {
			if (validationError instanceof ValidationError) {
				setGuestNameError(validationError.message);
			} else {
				setGuestNameError(t('errors.inputValidGuestName'));
			}
		}
	};

	const handleChange = async (e: ChangeEvent<HTMLInputElement>) => {
		const value = e.target.value;
		setGuestName(value);
		validateGuestField(value);
	};

	const guestNameStylesConfig = useMemo(
		() => ({
			label: {
				background: theme.palette['background-white'],
			},
			errorText: {
				wrapper: {
					background: theme.palette['background-white'],
				},
			},
		}),
		[theme]
	);

	useEffect(() => {
		if (error) {
			validateGuestField(guestName || '');
		}
	}, [error]);

	return (
		<BookingGuestInput
			id="guestName"
			name={nameLabel}
			onChange={handleChange}
			label={nameLabel}
			size="fullWidth"
			stylesConfig={guestNameStylesConfig}
			value={guestName || ''}
			isRequired
			error={!!guestNameError}
			errorText={guestNameError as string}
		/>
	);
}
