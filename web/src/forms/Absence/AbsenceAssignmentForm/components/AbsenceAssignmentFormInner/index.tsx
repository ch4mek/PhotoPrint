import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { SearchInput } from '@mms/mms-ui-library/components/SearchInput';
import { useEffect, useMemo } from 'react';

import {
	absenceAssignmentFieldData,
	ASSIGNMENT_FORM_TITLE,
	DISABLE_TOOLTIP_TEXT,
	formSections,
} from '../../constants';
import { AbsenceFormSectionWrapper, FormContent } from '../../styles';
import { TransferList } from '../TransferList';

import { FormSection } from '@/components/FormSection';
import { PositiveActionButton } from '@/components/PositiveActionButton';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { AbsenceType } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

import { AbsenceAssignmentFormInnerProps } from './types';

const { absence: absenceSectionTitle, employees } = formSections;

export function AbsenceAssignmentFormInner({
	formikProps,
	setSelectedAbsenceId,
	setSelectedAgreementId,
	absenceItems,
	absencesOptions,
	agreementOptions,
	isAgreementMandatory,
	isLoading,
	isApplyDisabled,
	handleClose,
	popUpStylesConfig,
	searchValue,
	handleClearSearch,
	handleSearchChange,
	debouncedSearchValue,
	unassignedUsers,
	assignedUsers,
	setAssignedUsers,
	setUnassignedUsers,
	checkedUsers,
	setCheckedUsers,
}: AbsenceAssignmentFormInnerProps) {
	const {
		submitForm,
		isSubmitting,
		setFieldValue,
		values,
		errors,
		touched,
		setFieldTouched,
		validateField,
		handleOutsideClose,
	} = formikProps;
	const { absence, agreement } = values;

	useEffect(() => {
		setSelectedAbsenceId(absence?.id || null);
	}, [absence]);

	useEffect(() => {
		setSelectedAgreementId(agreement?.id || null);
	}, [agreement]);

	const handleChange =
		(field: string) =>
		(value: SimpleConfigurableConfigurationItem<number> | null) => {
			if (field === absenceAssignmentFieldData.absence.name) {
				const absenceType = absenceItems.find(
					({ id }) => id === value?.id
				)?.type;

				if (
					absenceType !== AbsenceType.SickDay &&
					absenceType !== AbsenceType.Vacation
				) {
					setFieldValue(
						absenceAssignmentFieldData.effectiveFromDate.name,
						null
					);
					setFieldTouched(
						absenceAssignmentFieldData.effectiveFromDate.name,
						false
					);
				}

				setFieldValue(absenceAssignmentFieldData.agreement.name, null);
				setFieldTouched(absenceAssignmentFieldData.agreement.name, false);
			}

			setFieldValue(field, value);
			setFieldTouched(field, true);
			validateField(field);
		};

	const handleDateChange = (fieldName: string) => (value: Dayjs | null) => {
		if (value === null) {
			setFieldValue(fieldName, null, true);

			return;
		}

		setFieldValue(fieldName, value.toDate(), true);
		setFieldTouched(fieldName, true);
		validateField(fieldName);
	};

	const positiveControl = useMemo(
		() => ({
			customControl: (
				<PositiveActionButton
					onClick={submitForm}
					disabled={isApplyDisabled}
					tooltipText={DISABLE_TOOLTIP_TEXT}
				/>
			),
		}),
		[submitForm, isApplyDisabled]
	);

	return (
		<PopUp
			type="form"
			showLoader={isSubmitting || isLoading}
			title={ASSIGNMENT_FORM_TITLE}
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl,
				negativeControl: { onClick: handleClose },
			}}
			stylesConfig={popUpStylesConfig}
		>
			<FormContent>
				<FormSection title={absenceSectionTitle}>
					<AbsenceFormSectionWrapper>
						<Autoselect
							{...absenceAssignmentFieldData.absence}
							isRequired
							disableClearable
							value={absence}
							onChange={handleChange(absenceAssignmentFieldData.absence.name)}
							options={absencesOptions}
							errorText={errors.absence}
							error={Boolean(errors.absence) && touched.absence}
							getKey={extractKeyById}
							checkIfOptionEqualToValue={checkIfOptionsEqualById}
						/>
						{isAgreementMandatory && (
							<>
								<Autoselect
									{...absenceAssignmentFieldData.agreement}
									disableClearable
									value={values.agreement}
									onChange={handleChange(
										absenceAssignmentFieldData.agreement.name
									)}
									options={agreementOptions}
									errorText={errors.agreement}
									error={Boolean(errors.agreement) && touched.agreement}
									getKey={extractKeyById}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
									isRequired
								/>

								<DatePicker
									{...absenceAssignmentFieldData.effectiveFromDate}
									errorText={
										touched.effectiveFromDate && errors.effectiveFromDate
											? String(errors.effectiveFromDate)
											: ''
									}
									value={dayjs(values.effectiveFromDate)}
									onChange={handleDateChange(
										absenceAssignmentFieldData.effectiveFromDate.name
									)}
									size="fullWidth"
									maxDate={dayjs()}
									isRequired
								/>
							</>
						)}
					</AbsenceFormSectionWrapper>
				</FormSection>

				<FormSection title={employees}>
					<SearchInput
						value={searchValue}
						onDeleteIconClick={handleClearSearch}
						onChange={handleSearchChange}
						size="fullWidth"
					/>

					<TransferList
						unselectedItems={unassignedUsers}
						selectedItems={assignedUsers}
						setSelectedItems={setAssignedUsers}
						setUnselectedItems={setUnassignedUsers}
						searchValue={debouncedSearchValue}
						checkedItems={checkedUsers}
						setCheckedItems={setCheckedUsers}
					/>
				</FormSection>
			</FormContent>
		</PopUp>
	);
}
