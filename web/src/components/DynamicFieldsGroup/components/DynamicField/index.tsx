import { DynamicFieldWrapper } from '@/components/DynamicFieldsGroup/components/DynamicField/styles';
import { DynamicFieldProps } from '@/components/DynamicFieldsGroup/components/DynamicField/types';
import { TextField } from '@/fields/Text';

export function DynamicField({
	label,
	value,
	name,
	onChange,
	isError,
	isRequired,
}: DynamicFieldProps) {
	return (
		<DynamicFieldWrapper>
			<TextField
				width="fullWidth"
				label={label}
				value={value}
				name={name}
				onChange={onChange}
				error={isError}
				isRequired={isRequired}
			/>
		</DynamicFieldWrapper>
	);
}
