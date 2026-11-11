import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useMemo, useState } from 'react';

import { useJiraSettingsForm } from '../../hooks/useJiraSettingsForm';
import { usePopupStylesConfig } from '../../hooks/usePopupStylesConfig';

import { Divider } from '@/components/Divider';
import { useAuth } from '@/context/AuthContext';
import { TextField } from '@/fields/Text';
import { MASKED_PAT_PLACEHOLDER } from '@/forms/JiraSettingsForm/constants';
import { FormWrapper } from '@/forms/styles';
import { extractKeyById } from '@/helpers/extractKey';
import { useUserProjectsLookup } from '@/queries/useUserProjectsLookup';

import { DIVIDER_COLOR, jiraSettingsFormFields } from './constants';
import { FormRow } from './styles';
import { JiraSettingsFormContentProps } from './type';

export function JiraSettingsFormContent({
	onClose,
	isLoading,
	handleOutsideClose,
	popUpTitle,
	isEditing,
}: JiraSettingsFormContentProps) {
	const { id: userId } = useAuth();
	const {
		values,
		handleFieldChange,
		handleSubmit,
		setFieldValue,
		errors,
		touched,
	} = useJiraSettingsForm();

	const { data: projects = [], isLoading: isProjectsLoading } =
		useUserProjectsLookup(userId);

	const popUpStylesConfig = usePopupStylesConfig();
	const [isPatFocused, setIsPatFocused] = useState(false);

	const projectOptions = useMemo(
		() =>
			projects
				?.filter(({ isActive }) => isActive)
				.map(({ projectId, projectName }) => ({
					id: projectId,
					name: projectName,
				})),
		[projects]
	);

	const showLoader = isLoading || isProjectsLoading;
	const isMaskedPatShown =
		isEditing && !values.personalAccessToken && !isPatFocused;
	const maskedPatValueProps = isMaskedPatShown
		? { value: MASKED_PAT_PLACEHOLDER }
		: {};

	const handlePatFocus = useCallback(() => {
		setIsPatFocused(true);
	}, []);

	const handlePatBlur = useCallback(() => {
		setIsPatFocused(false);
	}, []);

	return (
		<PopUp
			type="form"
			title={popUpTitle}
			stylesConfig={popUpStylesConfig}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: onClose },
			}}
			onShadowClick={handleOutsideClose}
			{...{ onClose, showLoader }}
		>
			<FormWrapper>
				<TextField
					{...jiraSettingsFormFields.url}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...jiraSettingsFormFields.email}
					onChange={setFieldValue}
					isRequired
				/>
				<TextField
					{...jiraSettingsFormFields.personalAccessToken}
					onChange={setFieldValue}
					isRequired={!isEditing}
					onFocus={handlePatFocus}
					onBlur={handlePatBlur}
					{...maskedPatValueProps}
				/>
				<FormRow>
					<TextField
						{...jiraSettingsFormFields.projectTag}
						onChange={setFieldValue}
						isRequired
					/>
					<Divider color={DIVIDER_COLOR} />
					<Autoselect
						{...jiraSettingsFormFields.project}
						value={values.project}
						getKey={extractKeyById}
						options={projectOptions}
						onChange={handleFieldChange(jiraSettingsFormFields.project.name)}
						isRequired
						error={!!errors.project && touched.project}
						errorText={errors.project}
					/>
				</FormRow>
			</FormWrapper>
		</PopUp>
	);
}
