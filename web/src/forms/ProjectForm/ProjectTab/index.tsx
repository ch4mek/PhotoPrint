import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { RadioButton } from '@mms/mms-ui-library/components/RadioButton';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useFormikContext } from 'formik';
import { useCallback, useLayoutEffect, useState } from 'react';

import { CAPITALIZED_PROJECT_FORM_NAME, PROJECT_FORM_NAME } from '../constants';
import { ProjectFormValues } from '../types';

import { ActivationMessage } from '@/components/ActivationMessage';
import { COLOR_PICKER_FIELD_NUMBER_OF_COLORS } from '@/components/ColorPickerControl/constants';
import {
	getActivatingPopUpTitle,
	getDeactivatingPopUpTitle,
} from '@/constants/activityStatus';
import { popUpBreakpoints } from '@/constants/popUp';
import { isGroupManager, isHeadOrGroupManager } from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { ColorPickerField } from '@/fields/ColorPickerField';
import { TextField } from '@/fields/Text';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { useClientsLookup } from '@/queries/useClientsLookup';
import { useProject } from '@/queries/useProject';
import { useProjectManagersLookup } from '@/queries/useProjectManagersLookup';

import {
	IS_BILLABLE_GAP,
	IS_PUBLIC_GAP,
	PROJECT_FORM_FIELDS,
} from './constants';
import {
	ColorsFieldsWrapper,
	FormContentWrapper,
	InputFieldsWrapper,
	ProjectTypeWrapper,
	RadioButtonsWrapper,
	RadioFieldsWrapper,
} from './styles';
import { ProjectTabProps } from './types';

export function ProjectTab({
	handleClose,
	onChangeStatus,
	id,
	isActivityFormOpen,
	closeActivityForm,
}: ProjectTabProps) {
	const [isFormDisabled, setIsFormDisabled] = useState(false);
	const isMobileLWidth = useHasScreenWidth(popUpBreakpoints.mobileL);

	const { role } = useAuth();
	const isHeadOfDepartmentOrGroupManager = isHeadOrGroupManager(role);

	const {
		values,
		touched,
		errors,
		setFieldValue,
		setFieldTouched,
		validateField,
	} = useFormikContext<ProjectFormValues>();

	const { data: projectData } = useProject(id);

	const { data: clientOptions } = useClientsLookup({
		isDeleted: false,
		isActive: true,
	});

	const { data: projectManagersList } = useProjectManagersLookup({});

	useLayoutEffect(() => {
		if (projectData) {
			setIsFormDisabled(() => !projectData.isActive || isGroupManager(role));
		}
	}, [projectData]);

	const handleOnChange = (field: string) => (value: unknown) => {
		setFieldValue(field, value);
		setFieldTouched(field, true);
		validateField(field);
	};

	const handleEnableIsPublic = () => {
		setFieldValue(PROJECT_FORM_FIELDS.isPublic.name, true);

		if (values.isBillable) {
			setFieldValue(PROJECT_FORM_FIELDS.isBillable.name, false);
		}
	};

	const handleDisableIsPublic = () => {
		setFieldValue(PROJECT_FORM_FIELDS.isPublic.name, false);
	};

	const handleEnableIsBillable = () => {
		setFieldValue(PROJECT_FORM_FIELDS.isBillable.name, true);
	};

	const handleDisableIsBillable = () => {
		setFieldValue(PROJECT_FORM_FIELDS.isBillable.name, false);
	};

	const handleClick = useCallback(
		(fieldName: string, color: string) => setFieldValue(fieldName, color),
		[]
	);

	const handleChangeStatus = (status: boolean) => async () => {
		try {
			await onChangeStatus({
				id,
				isActive: !status,
				name: '',
			});
			handleClose();
		} finally {
			closeActivityForm();
		}
	};

	return (
		<FormContentWrapper>
			<InputFieldsWrapper>
				<TextField
					size="fullWidth"
					{...PROJECT_FORM_FIELDS.name}
					disabled={isFormDisabled}
					onChange={setFieldValue}
					isRequired
				/>
				<Autoselect
					{...PROJECT_FORM_FIELDS.client}
					value={values.client}
					valueField="name"
					options={
						(clientOptions || []) as unknown as {
							id: number;
							name: string;
						}[]
					}
					onChange={handleOnChange(PROJECT_FORM_FIELDS.client.name)}
					error={Boolean(touched.client && errors.client)}
					errorText={errors.client as string}
					disabled={isFormDisabled}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					isRequired
				/>
				<Autoselect
					{...PROJECT_FORM_FIELDS.manager}
					value={values.manager}
					valueField="fullName"
					options={projectManagersList || []}
					onChange={handleOnChange(PROJECT_FORM_FIELDS.manager.name)}
					error={Boolean(errors.manager) && touched.manager}
					errorText={errors.manager}
					disabled={isFormDisabled || isHeadOfDepartmentOrGroupManager}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
				/>
			</InputFieldsWrapper>
			<RadioFieldsWrapper>
				<ProjectTypeWrapper>
					<Typography variant="s-400">
						{PROJECT_FORM_FIELDS.isPublic.title}
					</Typography>
					<RadioButtonsWrapper gap={IS_PUBLIC_GAP}>
						<RadioButton
							onChange={handleEnableIsPublic}
							checked={values.isPublic}
							forAttribute={PROJECT_FORM_FIELDS.isPublic.positiveLabel}
							label={PROJECT_FORM_FIELDS.isPublic.positiveLabel}
							name={PROJECT_FORM_FIELDS.isPublic.name}
							disabled={isFormDisabled}
						/>
						<RadioButton
							onChange={handleDisableIsPublic}
							checked={!values.isPublic}
							forAttribute={PROJECT_FORM_FIELDS.isPublic.negativeLabel}
							label={PROJECT_FORM_FIELDS.isPublic.negativeLabel}
							name={PROJECT_FORM_FIELDS.isPublic.name}
							disabled={isFormDisabled}
						/>
					</RadioButtonsWrapper>
				</ProjectTypeWrapper>
				<ProjectTypeWrapper>
					<Typography variant="s-400">
						{PROJECT_FORM_FIELDS.isBillable.title}
					</Typography>
					<RadioButtonsWrapper gap={IS_BILLABLE_GAP}>
						<RadioButton
							onChange={handleEnableIsBillable}
							checked={values.isBillable}
							forAttribute={PROJECT_FORM_FIELDS.isBillable.positiveLabel}
							label={PROJECT_FORM_FIELDS.isBillable.positiveLabel}
							name={PROJECT_FORM_FIELDS.isBillable.name}
							disabled={values.isPublic || isFormDisabled}
						/>
						<RadioButton
							onChange={handleDisableIsBillable}
							checked={!values.isBillable}
							forAttribute={PROJECT_FORM_FIELDS.isBillable.negativeLabel}
							label={PROJECT_FORM_FIELDS.isBillable.negativeLabel}
							name={PROJECT_FORM_FIELDS.isBillable.name}
							disabled={values.isPublic || isFormDisabled}
						/>
					</RadioButtonsWrapper>
				</ProjectTypeWrapper>
			</RadioFieldsWrapper>
			<ColorsFieldsWrapper>
				<ColorPickerField
					value={values.color}
					name={PROJECT_FORM_FIELDS.color.name}
					disabled={isFormDisabled}
					flexible
					spaceBetween
					onChange={handleClick}
					numberOfColors={
						isMobileLWidth
							? COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobileL
							: COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobile
					}
				/>
			</ColorsFieldsWrapper>
			{isActivityFormOpen && (
				<PopUp
					title={
						values.isActive
							? getDeactivatingPopUpTitle(CAPITALIZED_PROJECT_FORM_NAME)
							: getActivatingPopUpTitle(CAPITALIZED_PROJECT_FORM_NAME)
					}
					onClose={closeActivityForm}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: {
							onClick: handleChangeStatus(projectData?.isActive ?? false),
						},
						negativeControl: { onClick: closeActivityForm },
					}}
					zIndex={zIndexes.formPopup}
				>
					<ActivationMessage
						name={projectData?.name}
						isActive={values.isActive}
						entityName={PROJECT_FORM_NAME}
					/>
				</PopUp>
			)}
		</FormContentWrapper>
	);
}
