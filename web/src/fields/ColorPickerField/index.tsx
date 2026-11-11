import { Field, FieldProps } from 'formik';

import { ColorPickerControl } from '@/components/ColorPickerControl';

import type { ColorPickerFieldProps } from './types';

export function ColorPickerField({
	name,
	...otherProps
}: ColorPickerFieldProps) {
	return (
		<Field name={name}>
			{({ form: { setFieldValue } }: FieldProps) => (
				<ColorPickerControl
					onChange={setFieldValue}
					fieldName={name}
					{...otherProps}
				/>
			)}
		</Field>
	);
}
