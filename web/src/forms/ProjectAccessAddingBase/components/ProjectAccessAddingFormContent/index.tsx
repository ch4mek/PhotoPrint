import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Switch } from '@mms/mms-ui-library/components/Switch';
import { ChangeEvent, useCallback, useMemo } from 'react';

import {
	DoubleColumnContainer,
	SwitchContainer,
	SwitchLabel,
} from '../../styles';
import { ProjectAccessFormFields } from '../../types';
import { getPopUpStylesConfig } from '../../utils';
import { FTEInput } from '../FTEInput';

import { useConfiguration } from '@/context/Configuration';
import { FormWrapper } from '@/forms/styles';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { ProjectLookupResponse } from '@/types/project';

import { ProjectAccessAddingFormContentProps } from './types';

export const ProjectAccessAddingFormContent = ({
	formikProps,
	formDisableFlags,
	options,
	label,
	projectSubValueField,
	isLoading,
	isAssignEditable,
	onClose,
}: ProjectAccessAddingFormContentProps) => {
	const {
		values,
		setFieldValue,
		errors,
		touched,
		setFieldTouched,
		validateField,
		submitForm,
		handleOutsideClose,
		setValues,
	} = formikProps;

	const {
		user,
		project,
		billability,
		startDate,
		endDate,
		fte,
		projectSeniority,
	} = values;

	const {
		project: isProjectDisabled,
		startDate: isStartDateDisabled,
		endDate: isEndDateDisabled,
		billability: isBillabilityDisabled,
		fte: isFteDisabled,
		projectSeniority: isSeniorityDisabled,
		user: isUserDisabled,
	} = formDisableFlags;

	const theme = useTheme();
	const [, windowHeight] = useResize();

	const {
		configurableValues: { seniorityLevels },
		isLoading: isConfigurationLoading,
	} = useConfiguration();

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const handleOnChange = (field: string) => (value: unknown) => {
		setFieldValue(field, value);
		setFieldTouched(field, true);
		validateField(field);
	};

	const handleProjectChange = useCallback((value: ProjectLookupResponse | null) => {
		const isProjectBillable = value?.isBillable ?? true;

		setFieldTouched(ProjectAccessFormFields.PROJECT, true);

		setValues((prevValues) => ({
			...prevValues,
			[ProjectAccessFormFields.PROJECT]: value,
			[ProjectAccessFormFields.BILLABILITY]: Boolean(isProjectBillable),
		}));
	}, []);

	const handleDateChange = useCallback(
		(fieldName: string) => async (value: Dayjs | null) => {
			await setFieldValue(fieldName, value, true);

			setFieldTouched(fieldName, true);
		},
		[]
	);

	const handleBillabilityChange = useCallback(
		(e: ChangeEvent<HTMLInputElement>, { checked }: { checked: boolean }) => {
			setFieldValue(ProjectAccessFormFields.BILLABILITY, Boolean(checked));
		},
		[]
	);

	const handleFTEChange = useCallback((value: string) => {
		setFieldValue(ProjectAccessFormFields.FTE, value, true);
		setFieldTouched(ProjectAccessFormFields.FTE, true);
	}, []);

	return (
		<PopUp
			title={label}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			type="form"
			controls={{
				positiveControl: {
					onClick: submitForm,
				},
				negativeControl: { onClick: onClose },
			}}
			showLoader={isLoading || isConfigurationLoading}
			stylesConfig={popUpStylesConfig}
		>
			<FormWrapper>
				<Autoselect
					label="User name"
					value={user}
					valueField="fullName"
					disabled={isUserDisabled || !isAssignEditable}
					options={options.users}
					onChange={handleOnChange(ProjectAccessFormFields.USER)}
					error={Boolean(errors.user) && touched.user}
					errorText={errors.user}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					isRequired
				/>
				<Autoselect
					label="Project name"
					value={project}
					valueField="name"
					disabled={isProjectDisabled || !isAssignEditable}
					options={options.projects}
					onChange={handleProjectChange}
					error={Boolean(errors.project) && touched.project}
					errorText={errors.project}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					isRequired
					subValueField={projectSubValueField}
				/>
				<DoubleColumnContainer>
					<DatePicker
						disabled={isStartDateDisabled}
						innerLabel="Start date"
						size="fullWidth"
						errorText={
							touched.startDate && errors.startDate
								? String(errors.startDate)
								: ''
						}
						value={dayjs(startDate)}
						onChange={handleDateChange(ProjectAccessFormFields.START_DATE)}
						isRequired
					/>
					<DatePicker
						disabled={isEndDateDisabled}
						innerLabel="End date"
						size="fullWidth"
						errorText={
							touched.endDate && errors.endDate ? String(errors.endDate) : ''
						}
						value={dayjs(endDate)}
						onChange={handleDateChange(ProjectAccessFormFields.END_DATE)}
					/>
				</DoubleColumnContainer>
				<DoubleColumnContainer>
					<Autoselect
						disabled={isSeniorityDisabled || !isAssignEditable}
						label="Project seniority"
						value={projectSeniority}
						valueField="name"
						options={seniorityLevels}
						onChange={handleOnChange(ProjectAccessFormFields.PROJECT_SENIORITY)}
						error={Boolean(errors.projectSeniority) && touched.projectSeniority}
						errorText={errors.projectSeniority}
						getKey={extractKeyById}
						checkIfOptionEqualToValue={checkIfOptionsEqualById}
						isRequired
					/>
					<FTEInput
						disabled={isFteDisabled || !isAssignEditable}
						size="fullWidth"
						label="FTE"
						name="fte"
						value={String(fte)}
						onChange={handleFTEChange}
						error={touched.fte && Boolean(errors.fte)}
						errorText={errors.fte}
						isRequired
					/>
				</DoubleColumnContainer>
				<SwitchContainer>
					<SwitchLabel
						disabled={isBillabilityDisabled || !isAssignEditable}
						variant="s-400"
					>
						Billable hours
					</SwitchLabel>
					<Switch
						disabled={isBillabilityDisabled || !isAssignEditable}
						name="billability"
						value="Billability"
						checked={billability}
						onChange={handleBillabilityChange}
					/>
				</SwitchContainer>
			</FormWrapper>
		</PopUp>
	);
};
