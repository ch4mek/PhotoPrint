import { useQueryClient } from '@tanstack/react-query';

import { timesheetQueryKeysToInvalidate } from '../constants';
import { InitialDataValues, JiraImportFormValues } from '../types';
import { convertFormValues } from '../utils/convertFormValues';

import {
	JIRA_IMPORT_SUCCESS_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useImportJiraTimesheets } from '@/queries/jiraImport/useImportJiraTimesheets';

export const useJiraImportForm = (handleClose: VoidFunction) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { mutateAsync, isPending } = useImportJiraTimesheets({
		onSuccess: () => {
			toast.open(
				createToast(NotificationType.SUCCESS, JIRA_IMPORT_SUCCESS_MESSAGE)
			);
			handleClose();

			timesheetQueryKeysToInvalidate.forEach((queryKey) => {
				queryClient.invalidateQueries({ queryKey: [queryKey] });
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const handleSubmit = async (values: InitialDataValues) => {
		await mutateAsync(convertFormValues(values as JiraImportFormValues));
	};

	return {
		handleSubmit,
		isSubmitting: isPending,
	};
};
