import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Button } from '@mms/mms-ui-library/components/Button';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { useMemo } from 'react';

import { popupFormKeys } from '../../constants';
import { FormDirtyTracker } from '../../FormDirtyTracker';
import { EquipmentHistory } from '../History';
import { Wrapper } from '../styled';

import { controlsText } from '@/constants/controlsTexts';
import { checkIfPrimitiveOptionsEqual } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyFromPrimitive } from '@/helpers/extractKey';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useEquipmentUsersLookup } from '@/queries/equipment/useEquipmentUsersLookup';

import { userFormFields } from './constants';
import { DatePickerWrapper, FormWrapper } from './styles';
import { UserEquipmentFormProps } from './types';
import { getAssignmentApproversOptions, getUsersOptions } from './utils';

export function UserEquipmentForm({
	formik,
	isBrokenStatus,
	isEquipmentAssigned,
	isEquipmentReturned,
	equipmentHistoryProps,
}: UserEquipmentFormProps) {
	const {
		values,
		setFieldValue,
		setFieldTouched,
		validateField,
		errors,
		touched,
		handleSubmit,
		resetForm,
	} = formik;

	const { data: allActiveUsers = [] } = useEquipmentUsersLookup();

	const assigmentApproversOptions = useMemo(
		() => getAssignmentApproversOptions(allActiveUsers, values),
		[allActiveUsers, values]
	);

	const assigmentApproverValue = assigmentApproversOptions.find(
		(approver) => approver.id === values.approver?.id
	);

	const usersOptions = useMemo(
		() => getUsersOptions(allActiveUsers, assigmentApproverValue),
		[allActiveUsers, assigmentApproverValue]
	);

	const isFormDisabled =
		isBrokenStatus || (isEquipmentAssigned && !isEquipmentReturned);
	const isReturnDateDisabled =
		isBrokenStatus || !isEquipmentAssigned || isEquipmentReturned;

	const handleDateChange = (field: string) => async (value: Dayjs | null) => {
		await setFieldValue(field, value || null);
		setFieldTouched(field, true);
		validateField(field);
	};

	const handleAutoselectChange = (field: string) => async (value: unknown) => {
		await setFieldValue(field, value);
		setFieldTouched(field, true);
		validateField(field);
	};

	const handleFormSubmit = () => {
		handleSubmit();
	};

	const equipmentProps = { ...equipmentHistoryProps, resetForm };

	return (
		<Wrapper>
			<FormDirtyTracker formKey={popupFormKeys.user} />
			<FormWrapper>
				<Autoselect
					{...userFormFields.approver}
					disabled={isFormDisabled}
					value={values.approver}
					options={assigmentApproversOptions}
					onChange={handleAutoselectChange(userFormFields.approver.name)}
					error={Boolean(errors.approver && touched.approver)}
					errorText={errors.approver}
					getKey={extractKeyFromPrimitive}
					checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
					isRequired
				/>

				<Autoselect
					{...userFormFields.user}
					value={values.selectedUser}
					options={usersOptions}
					onChange={handleAutoselectChange(userFormFields.user.name)}
					error={Boolean(errors.selectedUser && touched.selectedUser)}
					errorText={errors.selectedUser}
					disabled={isFormDisabled}
					getKey={extractKeyFromPrimitive}
					checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
					isRequired
				/>

				<DatePickerWrapper>
					<DatePicker
						{...userFormFields.issueDate}
						value={values.issueDate ? dayjs(values.issueDate) : null}
						onChange={handleDateChange(userFormFields.issueDate.name)}
						disabled={isFormDisabled}
						errorText={
							touched.issueDate && errors.issueDate
								? String(errors.issueDate)
								: ''
						}
						isRequired
						size="fullWidth"
					/>

					<DatePicker
						{...userFormFields.returnDate}
						value={values.returnDate ? dayjs(values.returnDate) : null}
						onChange={handleDateChange(userFormFields.returnDate.name)}
						errorText={
							touched.returnDate && errors.returnDate
								? String(errors.returnDate)
								: ''
						}
						disabled={isReturnDateDisabled}
						size="fullWidth"
					/>
				</DatePickerWrapper>
				<Button
					isFullWidth={false}
					disabled={isBrokenStatus}
					onClick={handleFormSubmit}
				>
					{controlsText.applyText}
				</Button>
			</FormWrapper>
			<EquipmentHistory {...equipmentProps} />
		</Wrapper>
	);
}
