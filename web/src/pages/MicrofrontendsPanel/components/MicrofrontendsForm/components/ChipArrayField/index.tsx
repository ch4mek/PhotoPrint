import { useTheme } from '@emotion/react';
import { Chip } from '@mms/mms-ui-library/components/Chip';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { useFormikContext } from 'formik';
import { useCallback, useState } from 'react';

import { MicrofrontendsFormValues } from '../../types';

import { EVENT_ENTER_KEY } from '@/constants/eventKeys';
import { TextField } from '@/fields/Text';

import { CHIP_ARRAY_FIELD_ERRORS, CHIP_ARRAY_FIELD_LABELS } from './constants';
import {
	AddButton,
	ChipArrayFieldWrapper,
	ChipsContainer,
	InputWithButtonWrapper,
} from './styles';
import { ChipArrayFieldProps } from './types';

export function ChipArrayField({
	name,
	label,
	maxItems,
	validation,
	isRequired = false,
}: ChipArrayFieldProps) {
	const theme = useTheme();

	const { values, setFieldValue, errors, touched, setFieldTouched } =
		useFormikContext<MicrofrontendsFormValues>();
	const [inputValue, setInputValue] = useState('');
	const [inputError, setInputError] = useState<string | null>(null);

	const fieldValues: string[] = values[name] || [];
	const formikError = touched[name] && errors[name];

	const handleAdd = useCallback(() => {
		const trimmedValue = inputValue.trim();

		if (!trimmedValue) {
			setInputError(CHIP_ARRAY_FIELD_ERRORS.EMPTY_VALUE);

			return;
		}

		if (maxItems && fieldValues.length >= maxItems) {
			setInputError(CHIP_ARRAY_FIELD_ERRORS.MAX_ITEMS_REACHED(maxItems));

			return;
		}

		if (validation) {
			const error = validation(trimmedValue);

			if (error) {
				setInputError(error);

				return;
			}
		}

		if (fieldValues.includes(trimmedValue)) {
			setInputError(CHIP_ARRAY_FIELD_ERRORS.DUPLICATE_VALUE);

			return;
		}

		const newValues = [...fieldValues, trimmedValue];
		setFieldValue(name, newValues);
		setFieldTouched(name, true);
		setInputValue('');
		setInputError(null);
	}, [
		inputValue,
		fieldValues,
		name,
		setFieldValue,
		setFieldTouched,
		validation,
		maxItems,
	]);

	const handleRemove = useCallback(
		(indexToRemove: number) => {
			const newValues = fieldValues.filter(
				(_, index) => index !== indexToRemove
			);
			setFieldValue(name, newValues.length > 0 ? newValues : null);
			setFieldTouched(name, true);
		},
		[fieldValues, name, setFieldValue, setFieldTouched]
	);

	const handleKeyPress = useCallback(
		(e: React.KeyboardEvent<HTMLInputElement>) => {
			if (e.key === EVENT_ENTER_KEY) {
				e.preventDefault();
				handleAdd();
			}
		},
		[handleAdd]
	);

	const handleInputChange = useCallback((fieldName: string, value: string) => {
		setInputValue(value);
		setInputError(null);
	}, []);

	const isMaxReached = maxItems ? fieldValues.length >= maxItems : false;

	const displayError = inputError || (formikError ? String(errors[name]) : '');
	const hasError = !!displayError;

	return (
		<ChipArrayFieldWrapper>
			<InputWithButtonWrapper>
				<TextField
					name={`${name}_input`}
					label={label}
					value={inputValue}
					onChange={handleInputChange}
					onKeyDown={handleKeyPress}
					isRequired={isRequired}
					disabled={isMaxReached}
					errorText={displayError}
					error={hasError}
					style={{
						paddingRight: theme.spaces.xl,
					}}
				/>
				<AddButton
					type="button"
					onClick={handleAdd}
					disabled={!inputValue.trim() || isMaxReached}
					title={
						isMaxReached
							? CHIP_ARRAY_FIELD_LABELS.MAX_REACHED(maxItems!)
							: CHIP_ARRAY_FIELD_LABELS.ADD
					}
				>
					<PlusIcon color={theme.palette['text-white']} />
				</AddButton>
			</InputWithButtonWrapper>

			{fieldValues.length > 0 && (
				<ChipsContainer>
					{fieldValues.map((value, index) => (
						<Chip
							key={`${value}-${index}`}
							labelStyle={{ padding: theme.spaces.xs }}
							onDelete={() => handleRemove(index)}
							label={value}
							withCrossIcon
						/>
					))}
				</ChipsContainer>
			)}
		</ChipArrayFieldWrapper>
	);
}
