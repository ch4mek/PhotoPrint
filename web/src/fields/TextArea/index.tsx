import { TextArea as TextAreaField } from '@mms/mms-ui-library/components/TextArea';
import { Field, FieldProps } from 'formik';
import { ChangeEvent, useCallback } from 'react';
import { JSX } from 'react/jsx-runtime';

import type { MMSTextAreaProps } from './types';

export function TextArea({
	name,
	onChange: onChangeValue,
	ref,
	...otherProps
}: MMSTextAreaProps) {
	const handleOnChange = useCallback(
		(e: ChangeEvent<HTMLTextAreaElement>) => {
			if (onChangeValue) {
				onChangeValue(name, e.target.value);
			}
		},
		[onChangeValue]
	);

	return (
		<Field name={name}>
			{(props: JSX.IntrinsicAttributes & FieldProps<string>) => {
				const {
					field: { onChange },
					meta: { value, error, touched },
				} = props;

				const errorMessage = (value || touched) && error;

				return (
					<TextAreaField
						name={name}
						minRows={4}
						error={Boolean(errorMessage)}
						errorText={error}
						value={value}
						onChange={onChangeValue ? handleOnChange : onChange}
						ref={ref}
						{...otherProps}
					/>
				);
			}}
		</Field>
	);
}
