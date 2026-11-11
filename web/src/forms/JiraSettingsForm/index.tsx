import { useCallback, useMemo } from 'react';

import { FormikWithBackup } from '../components/FormikWithBackup';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	JIRA_SETTINGS_SAVED,
	NotificationType,
} from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useCreateJiraSettings } from '@/queries/jira/useCreateJiraSettings';
import { useUpdateJiraSettings } from '@/queries/jira/useUpdateJiraSettings';

import { JiraSettingsFormContent } from './components/JiraSettingsFormContent';
import { FORM_NAME, jiraSettingsFormInitialValues } from './constants';
import { getJiraSettingsFormSchema } from './schema';
import {
	JiraSettingsFormProps,
	JiraSettingsFormValues,
	ValidatedJiraSettingsFormValues,
} from './types';
import {
	getInitialValues,
	mapJiraSettingsFormValuesToCreateParams,
	mapJiraSettingsFormValuesToUpdateParams,
} from './utils';

export function JiraSettingsForm({
	onClose,
	jiraSettings,
}: JiraSettingsFormProps) {
	const { id: userId } = useAuth();
	const toast = useToast();
	const isEditing = !!jiraSettings;

	const initialValues = useMemo(
		() =>
			jiraSettings
				? getInitialValues(jiraSettings)
				: jiraSettingsFormInitialValues,
		[jiraSettings]
	);
	const validationSchema = useMemo(
		() => getJiraSettingsFormSchema(isEditing),
		[isEditing]
	);

	const { mutateAsync: createJiraSettings, isPending: isCreating } =
		useCreateJiraSettings({
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, JIRA_SETTINGS_SAVED));
				onClose();
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: updateJiraSettings, isPending: isUpdating } =
		useUpdateJiraSettings({
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, JIRA_SETTINGS_SAVED));
				onClose();
			},
			onError: createOnErrorHandler(toast),
		});

	const handleSubmit = useCallback(
		async (values: JiraSettingsFormValues) => {
			if (jiraSettings) {
				await updateJiraSettings(
					mapJiraSettingsFormValuesToUpdateParams(
						values as ValidatedJiraSettingsFormValues,
						userId,
						jiraSettings.id
					)
				);

				return;
			}

			await createJiraSettings(
				mapJiraSettingsFormValuesToCreateParams(
					values as ValidatedJiraSettingsFormValues,
					userId
				)
			);
		},
		[jiraSettings, createJiraSettings, updateJiraSettings, userId]
	);

	const popUpTitle = useMemo(
		() =>
			jiraSettings
				? getEditingPopUpTitle(FORM_NAME)
				: getAddingPopUpTitle(FORM_NAME),
		[jiraSettings]
	);

	return (
		<FormikWithBackup
			onSubmit={handleSubmit}
			name={FORM_NAME}
			id={jiraSettings?.id}
			validateOnBlur={false}
			enableReinitialize
			{...{ onClose, initialValues, validationSchema }}
		>
			{({ handleOutsideClose }) => (
				<JiraSettingsFormContent
					{...{
						onClose,
						isLoading: isCreating || isUpdating,
						handleOutsideClose,
						popUpTitle,
						isEditing,
					}}
				/>
			)}
		</FormikWithBackup>
	);
}
