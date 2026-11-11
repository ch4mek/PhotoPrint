import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { FieldArray, useFormikContext } from 'formik';
import { useCallback } from 'react';

import { AddMoreButton } from '../AddMoreButton';

import { DynamicField } from '@/components/DynamicFieldsGroup/components/DynamicField';
import { MAX_NUMBER_OF_FIELDS } from '@/components/DynamicFieldsGroup/constants';
import {
	DynamicFieldsGroupWrapper,
	DynamicFieldWrapper,
	FieldArrayWrapper,
} from '@/components/DynamicFieldsGroup/styles';
import { DynamicFieldsGroupProps } from '@/components/DynamicFieldsGroup/types';
import { getFieldLabel } from '@/components/DynamicFieldsGroup/utils/getFieldLabel';

export function DynamicFieldsGroup<T>({
	name,
	label,
	mainLabel,
	additionalLabel,
	gridArea,
	onChange,
	maxNumberOfFields = MAX_NUMBER_OF_FIELDS,
}: DynamicFieldsGroupProps) {
	const { values, setFieldValue, errors, touched } = useFormikContext<T>();
	const fieldsValues = (values[name as keyof T] as string[]) || [];

	const fieldsToRender = fieldsValues?.length > 0 ? fieldsValues : [''];
	const fieldErrors = errors[name as keyof T];
	const fieldTouched = touched[name as keyof T];

	const handleChangeValue = useCallback(
		(index: number) => (field: string, value: string) => {
			if (onChange) {
				onChange(field, value);

				return;
			}

			setFieldValue(`${name}.${index}`, value);
		},
		[setFieldValue]
	);

	return (
		<DynamicFieldsGroupWrapper gridArea={gridArea}>
			<FieldArray name={String(name)}>
				{({ push, remove }) => {
					const handleAddMoreField = () => push('');
					const handleRemoveField = (index: number) => () => remove(index);

					return (
						<>
							<FieldArrayWrapper>
								{fieldsToRender?.map((url: string, index: number) => (
									<DynamicFieldWrapper key={index}>
										<DynamicField
											label={getFieldLabel({
												index,
												label,
												mainLabel,
												additionalLabel,
											})}
											value={url}
											name={`${name}.${index}`}
											onChange={handleChangeValue(index)}
											isError={Boolean(
												Array.isArray(fieldTouched) &&
													fieldTouched[index] &&
													Array.isArray(fieldErrors) &&
													fieldErrors?.[index]
											)}
										/>
										{fieldsToRender.length > 1 && (
											<ActionButton onClick={handleRemoveField(index)}>
												<BasketIcon />
											</ActionButton>
										)}
									</DynamicFieldWrapper>
								))}
							</FieldArrayWrapper>

							{fieldsToRender?.length < maxNumberOfFields && (
								<AddMoreButton handleClick={handleAddMoreField} />
							)}
						</>
					);
				}}
			</FieldArray>
		</DynamicFieldsGroupWrapper>
	);
}
