import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import {
	FieldArray,
	FormikErrors,
	FormikTouched,
	useFormikContext,
} from 'formik';
import { useCallback, useMemo } from 'react';

import { getScopeOptions } from '../../utils/getScopeOptions';
import { getUserOptions } from '../../utils/getUserOptions';
import { DynamicField } from '../DynamicField';

import { AddMoreButton } from '@/components/AddMoreButton';
import { AbsenceFormValues } from '@/forms/Absence/AbsenceForm/types';
import { AcceptPolicesItem } from '@/types/absences';

import { initialAddMoreFieldValue, MAX_NUMBER_OF_FIELDS } from './constants';
import {
	ActionButtonWrapper,
	DynamicFieldsGroupWrapper,
	DynamicFieldWrapper,
	FieldArrayWrapper,
} from './styles';
import { DynamicFieldsGroupProps } from './types';

export function ApprovelsFieldsGroup({
	name,
	gridArea,
	users = [],
	maxNumberOfFields = MAX_NUMBER_OF_FIELDS,
}: DynamicFieldsGroupProps) {
	const { values, touched, errors, setFieldValue, setFieldTouched } =
		useFormikContext<AbsenceFormValues>();

	const fieldsValues = values.approvals?.acceptPolicies || [];
	const fieldErrors = errors.approvals?.acceptPolicies as
		| FormikErrors<AcceptPolicesItem>[]
		| undefined;
	const fieldTouched = touched.approvals?.acceptPolicies as
		| FormikTouched<AcceptPolicesItem>[]
		| undefined;

	const usersOptions = useMemo(
		() => getUserOptions(users, fieldsValues),
		[users, fieldsValues]
	);

	const getUsersOptions = (
		substituteUserId: string | number | null | undefined
	) => {
		if (!substituteUserId) {
			return usersOptions;
		}

		return usersOptions.filter((item) => item.id !== substituteUserId);
	};

	const handleChangeValue = useCallback(
		(index: number) => (value: AcceptPolicesItem) => {
			setFieldValue(
				`approvals.${name}.${index}`,
				{ ...value, orderWeight: index + 1 },
				true
			);
			setFieldTouched(`approvals.${name}.${index}`, true);
		},
		[setFieldValue, setFieldTouched, name]
	);

	const availableOptions = useMemo(
		() => getScopeOptions(fieldsValues),
		[fieldsValues]
	);

	return (
		<DynamicFieldsGroupWrapper gridArea={gridArea}>
			<FieldArray name={`approvals.${name}`}>
				{({ push }) => {
					const handleAddMoreField = () => {
						push({
							...initialAddMoreFieldValue,
							orderWeight: fieldsValues.length + 1,
						});
					};

					const handleRemoveField = (index: number) => () => {
						const arr = fieldsValues.slice();
						arr.splice(index, 1);

						const updated = arr.map((item, i) => ({
							...item,
							orderWeight: i + 1,
						}));

						setFieldValue(`approvals.${name}`, updated, false);
					};

					return (
						<>
							<FieldArrayWrapper>
								{fieldsValues.map((value: AcceptPolicesItem, index: number) => (
									<DynamicFieldWrapper
										key={`${name}-${index}-${value.orderWeight}`}
									>
										<DynamicField
											isShowIcon={index === 0}
											value={value}
											roleOptionsValues={availableOptions}
											usersOptions={getUsersOptions(value.substituteUserId)}
											onChange={handleChangeValue(index)}
											error={fieldErrors && fieldErrors[index]}
											fieldTouched={fieldTouched?.[index]}
										/>
										{index !== 0 && (
											<ActionButtonWrapper
												size="large"
												onClick={handleRemoveField(index)}
											>
												<BasketIcon />
											</ActionButtonWrapper>
										)}
									</DynamicFieldWrapper>
								))}
							</FieldArrayWrapper>

							{fieldsValues.length < maxNumberOfFields && (
								<AddMoreButton handleClick={handleAddMoreField} />
							)}
						</>
					);
				}}
			</FieldArray>
		</DynamicFieldsGroupWrapper>
	);
}
