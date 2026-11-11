import React, {
	ChangeEvent,
	useCallback,
	useEffect,
	useRef,
	useState,
} from 'react';
import { ValidationError } from 'yup';

import { useCalendarContextOperations } from '../../hooks/useCalendarOperations';

import {
	COUNTRY_ALREADY_EXISTS,
	COUNTRY_ALREADY_USED,
} from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/index';
import { useSettingsCalendarCountriesContext } from '@/context/SettingsCalendarCountriesContext';
import { textType1Schema } from '@/forms/schemas';
import { createToast } from '@/helpers/createToast';
import { useStopGridCellPropagation } from '@/hooks/useStopGridCellPropagation';

import { CountryNameInput, CountryNameInputWrapper } from './styles';
import { CountryNameCellProps } from './types';

const FIELD_NAME = 'country';

function CountryNameCellInner({
	countryId,
	countryName,
	rowIdx,
	error: externalError,
}: CountryNameCellProps) {
	const [error, setError] = useState<string | undefined>(undefined);
	const prevCountryNameRef = useRef(countryName);
	const toast = useToast();

	const countries = useSettingsCalendarCountriesContext();

	const { onCountryRowChange, updateCountry, deleteCountry } =
		useCalendarContextOperations();

	useEffect(() => {
		if (externalError !== error) {
			onCountryRowChange(rowIdx, {
				error,
			});
		}
	}, [error]);

	const validateCountryName = useCallback((newCountryName: string) => {
		textType1Schema(FIELD_NAME)
			.validate(newCountryName)
			.then(() => {
				setError(undefined);
			})
			.catch((validationError) => {
				if (
					validationError instanceof ValidationError &&
					validationError.inner
				) {
					setError(validationError.message);
				} else {
					setError(undefined);
				}
			});
	}, []);

	const handleCountryNameChange = (event: ChangeEvent<HTMLInputElement>) => {
		const newName = event.target.value;
		onCountryRowChange(rowIdx, {
			countryName: newName,
		});

		validateCountryName(newName);
	};

	const { handleClick, handleKeyDown } = useStopGridCellPropagation();

	const handleCountryNameBlur = () => {
		if (countryName === prevCountryNameRef.current || error) {
			return;
		}

		const hasEqualNames = countries
			? Object.entries(countries).some(
					([oldCountryId, oldCountryName]) =>
						oldCountryId !== String(countryId) &&
						oldCountryName.toLowerCase() === countryName.toLowerCase()
			  )
			: false;

		if (hasEqualNames) {
			setError(COUNTRY_ALREADY_USED);
			toast.open(createToast(NotificationType.ERROR, COUNTRY_ALREADY_EXISTS));

			return;
		}

		if (countryId && countryName === '') {
			deleteCountry(countryId);

			return;
		}

		if (countryId) {
			updateCountry(countryId, countryName, (updatedCountry) => {
				prevCountryNameRef.current = updatedCountry.name;
			});
		}
	};

	return (
		<CountryNameInputWrapper onClick={handleClick} onKeyDown={handleKeyDown}>
			<CountryNameInput
				size="fullWidth"
				value={countryName}
				label="Country"
				onChange={handleCountryNameChange}
				onBlur={handleCountryNameBlur}
				error={!!error}
				errorText={error}
				disabled
			/>
		</CountryNameInputWrapper>
	);
}

export const CountryNameCell = React.memo(CountryNameCellInner);
