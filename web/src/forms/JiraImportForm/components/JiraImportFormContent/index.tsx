import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePickerRange } from '@mms/mms-ui-library/components/CustomDatePickerRange';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback } from 'react';

import { fieldsNames, POPUP_TITLE } from '../../constants';
import { useProjectOptions } from '../../hooks/useProjectOptions';
import { JiraImportFormContentProps } from '../../types';

import { PositiveActionButton } from '@/components/PositiveActionButton';
import { UserProfileTabsId } from '@/components/UserProfile/types';
import { controlsText as commonControlsText } from '@/constants/controlsTexts';
import { jiraPopUpStylesConfig } from '@/constants/jira';
import { USER_PROFILE_TAB_ID_PARAM } from '@/constants/searchParams';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { JiraImportProjectOption } from '@/types/jiraImport';

import { controlsText, fieldData } from './constants';
import { FormWrapper } from './styles';

export function JiraImportFormContent({
	formikProps,
	isDisableForm,
	handleClose,
}: JiraImportFormContentProps) {
	const {
		values,
		errors,
		touched,
		submitForm,
		setFieldValue,
		setFieldTouched,
		handleOutsideClose,
		validateField,
	} = formikProps;
	const navigate = useCustomNavigate();

	const { projects, isFetchingProjects } = useProjectOptions();
	const isLoading = isFetchingProjects || isDisableForm;

	const handleDateChange = useCallback(
		async (newValue: [Dayjs | null, Dayjs | null]) => {
			if (newValue === null) {
				await setFieldValue(fieldsNames.dateFrom, null, true);
				await setFieldValue(fieldsNames.dateTo, null, true);

				return;
			}

			await setFieldValue(
				fieldsNames.dateFrom,
				newValue[0]?.toDate() || null,
				true
			);
			await setFieldValue(
				fieldsNames.dateTo,
				newValue[1]?.toDate() || null,
				true
			);
			setFieldTouched(fieldsNames.dateFrom, true);
			setFieldTouched(fieldsNames.dateTo, true);
			validateField(fieldsNames.dateFrom);
			validateField(fieldsNames.dateTo);
		},
		[setFieldValue, setFieldTouched, validateField]
	);

	const handleProjectChange = useCallback(
		async (option: JiraImportProjectOption | null) => {
			await setFieldValue(fieldsNames.project, option);
			setFieldTouched(fieldsNames.project, true);
			validateField(fieldsNames.project);
		},
		[setFieldValue, setFieldTouched, validateField]
	);

	const handleOpenSettings = useCallback(() => {
		handleClose();
		navigate({
			tabId: {
				key: USER_PROFILE_TAB_ID_PARAM,
				value: UserProfileTabsId.JIRA_SETTINGS,
			},
		});
	}, [handleClose, navigate]);

	return (
		<PopUp
			type="form"
			showLoader={isLoading}
			title={POPUP_TITLE}
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			stylesConfig={jiraPopUpStylesConfig}
			controls={{
				positiveControl: {
					customControl: (
						<PositiveActionButton
							disabled={isDisableForm}
							onClick={isLoading ? undefined : submitForm}
							text={controlsText.import}
						/>
					),
				},
				extraControls: [
					{
						color: 'secondary',
						text: controlsText.openSettings,
						onClick: handleOpenSettings,
					},
					{
						color: 'secondary',
						text: commonControlsText.cancelText,
						onClick: handleClose,
					},
				],
			}}
		>
			<FormWrapper>
				<DatePickerRange
					{...fieldData.dateRange}
					range={[
						values.dateFrom ? dayjs(values.dateFrom) : null,
						values.dateTo ? dayjs(values.dateTo) : null,
					]}
					onChange={handleDateChange}
					size="fullWidth"
					disabled={isDisableForm}
					error={
						(Boolean(errors.dateFrom) && Boolean(touched.dateFrom)) ||
						(Boolean(errors.dateTo) && Boolean(touched.dateTo))
					}
					errorText={
						errors.dateFrom || errors.dateTo
							? String(errors.dateFrom || errors.dateTo)
							: undefined
					}
					isRequired
				/>
				<Autoselect
					{...fieldData.project}
					value={values.project}
					key={fieldsNames.project}
					onChange={handleProjectChange}
					options={projects}
					error={Boolean(errors.project) && Boolean(touched.project)}
					disabled={isDisableForm}
					errorText={errors.project}
					getKey={extractKeyById}
					checkIfOptionEqualToValue={checkIfOptionsEqualById}
					shouldBeClosedOnOutsideScroll={false}
					disableClearable
					isRequired
				/>
			</FormWrapper>
		</PopUp>
	);
}
