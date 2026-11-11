import { Input } from '@mms/mms-ui-library/components/Input';
import { Field, FieldProps, useFormikContext } from 'formik';
import { useCallback, FocusEvent, ChangeEvent } from 'react';

import type { TextFieldProps } from './types';

export function TextField({
	label,
	name,
	onChange,
	onChangeAdditionalCallback,
	size = 'fullWidth',
	error: customError,
	errorText: customErrorText,
	onBlur,
	withTrimOnBlur,
	...otherProps
}: TextFieldProps) {
	const { setFieldTouched, setFieldValue } = useFormikContext();

	const handleBlur = useCallback(
		(event: FocusEvent<HTMLInputElement>) => {
			if (withTrimOnBlur) {
				const trimmed = event.target.value.trim();

				if (trimmed !== event.target.value) {
					setFieldValue(name, trimmed, true);
				}
			}

			onBlur?.(event);
		},
		[withTrimOnBlur, setFieldValue, name, onBlur]
	);

	const handleOnChange = useCallback(
		(e: ChangeEvent<HTMLInputElement>) => {
			onChange(name, e.target.value);
			setFieldTouched(name, true);

			if (onChangeAdditionalCallback) {
				onChangeAdditionalCallback();
			}
		},
		[onChange, name, setFieldTouched, onChangeAdditionalCallback]
	);

	return (
		<Field name={name}>
			{(props: FieldProps) => {
				const {
					meta: { value, error, touched },
				} = props;

				const errorMessage = (touched || value) && error;

				return (
					<Input
						id={name}
						name={name}
						onChange={handleOnChange}
						label={label ?? ''}
						size={size}
						onBlur={handleBlur}
						value={value || ''}
						error={customError || errorMessage}
						errorText={customErrorText || error}
						{...otherProps}
					/>
				);
			}}
		</Field>
	);
}
