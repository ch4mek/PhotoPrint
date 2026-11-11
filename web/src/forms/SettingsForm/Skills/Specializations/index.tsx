import { useTheme } from '@emotion/react';
import { AutoSelectWithChips } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import {
	getEditingPopUpTitle,
	getAddingPopUpTitle,
} from '@/constants/activityStatus';
import { isChiefRole, isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { findDepartmentById } from '@/forms/utils';
import { useResize } from '@/hooks/useResize';
import { useHardSkillsLookup } from '@/queries/settings/useHardSkillsLookup';
import { useSpecialization } from '@/queries/settings/useSpecialization';

import { DEPARTMENT_FALLBACK, entityName, FORM_NAME } from './constants';
import { validationSchema } from './schema';
import {
	SpecializationDepartmentOption,
	SpecializationFormProps,
	SpecializationFormValues,
} from './types';
import { createDepartmentDisabledMessage, getPopUpStylesConfig } from './utils';

export function SpecializationForm({
	handleClose,
	onSubmit,
	id,
}: SpecializationFormProps) {
	const { data, isLoading: isSpecializationLoading } = useSpecialization(id);
	const { role, departmentId } = useAuth();
	const {
		configurableValues: { departmentTypes: allDepartments },
	} = useConfiguration();
	const theme = useTheme();
	const [, windowHeight] = useResize();

	const isEditMode = Boolean(id);

	const handleSubmit = useCallback(
		async (
			values: SpecializationFormValues,
			{ setSubmitting }: FormikHelpers<SpecializationFormValues>
		) => {
			const isSubmited = await onSubmit(values);

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[]
	);

	const { data: hardSkills, isLoading: areHardSkillsLoading } =
		useHardSkillsLookup(true);

	const initialValues: SpecializationFormValues = useMemo(() => {
		if (!isEditMode || !data) {
			return { name: '', departments: [] };
		}

		return {
			...data,
			departments: data.departmentDtos.map(
				({
					departmentId: reciewedDepartmentId,
					isUserFromDepartmentAssigned,
				}) => {
					const disabledMessage = createDepartmentDisabledMessage(
						isUserFromDepartmentAssigned,
						isChiefRole(role) ? true : departmentId === reciewedDepartmentId
					);

					return {
						id: reciewedDepartmentId,
						fullName:
							findDepartmentById(allDepartments, reciewedDepartmentId)
								?.fullName || DEPARTMENT_FALLBACK,
						disabled: Boolean(disabledMessage),
						disabledMessage,
					};
				}
			),
		};
	}, [isEditMode, data, role]);

	const departmentsOptions: Array<SpecializationDepartmentOption> =
		useMemo(() => {
			if (!allDepartments.length) {
				return [];
			}

			const disabledDepartments = (initialValues.departments || [])
				.filter(({ disabled }) => disabled)
				.map(({ id: disabledDeparmentId, disabledMessage }) => ({
					id: disabledDeparmentId,
					disabledMessage,
				}));

			if (!isHeadOrGroupManager(role)) {
				return allDepartments.map((configurationDepartment) => {
					const disabledDepartment = disabledDepartments.find(
						(department) => department.id === configurationDepartment.id
					);

					return {
						...configurationDepartment,
						disabled: Boolean(disabledDepartment?.disabledMessage),
						disabledMessage: disabledDepartment?.disabledMessage,
					};
				});
			}

			const headDepartment = allDepartments.find(
				(department) => department.id === departmentId
			)!;

			const disabledDepartment = disabledDepartments.find(
				(department) => department.id === headDepartment.id
			);

			return [
				{
					...headDepartment,
					disabled: Boolean(disabledDepartment?.disabledMessage),
					disabledMessage: disabledDepartment?.disabledMessage,
				},
			];
		}, [allDepartments, role, departmentId, initialValues]);

	const checkIsDepartmentChipDisabled = useMemo(() => {
		if (!isEditMode) {
			return undefined;
		}

		return (chip: SpecializationDepartmentOption) => Boolean(chip.disabled);
	}, [isEditMode]);

	const getDisabledMessage = (option: SpecializationDepartmentOption) =>
		option.disabledMessage;

	const popupStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={id}
		>
			{({
				setFieldValue,
				setFieldTouched,
				submitForm,
				validateField,
				isSubmitting,
				values,
				errors,
				touched,
				handleOutsideClose,
			}) => {
				const handleOnChange =
					(field: 'departments' | 'hardSkills') => (value: unknown) => {
						setFieldValue(field, value);
						setFieldTouched(field, true);
						validateField(field);
					};

				return (
					<PopUp
						title={
							isEditMode
								? getEditingPopUpTitle(entityName)
								: getAddingPopUpTitle(entityName)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
						showLoader={
							isSpecializationLoading || areHardSkillsLoading || isSubmitting
						}
						stylesConfig={popupStylesConfig}
					>
						<FormWrapper>
							<TextField
								name="name"
								label="Specialization name"
								onChange={setFieldValue}
								isRequired
							/>
							<AutoSelectWithChips
								value={values?.departments || []}
								options={departmentsOptions}
								label="Department"
								valueField="fullName"
								onChange={handleOnChange('departments')}
								checkIfChipDisabled={checkIsDepartmentChipDisabled}
								errorText={errors.departments}
								error={Boolean(errors.departments) && touched.departments}
								getDisabledMessage={getDisabledMessage}
								isRequired
							/>

							<AutoSelectWithChips
								value={values?.hardSkills || []}
								options={hardSkills || []}
								label="Hard skills"
								onChange={handleOnChange('hardSkills')}
								valueField="name"
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
