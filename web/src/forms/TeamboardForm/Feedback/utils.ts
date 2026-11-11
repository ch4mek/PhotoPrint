import { Theme } from '@emotion/react';

import { popUpSizes } from '@/constants/popUp';
import { formatDateString } from '@/helpers/dateToISOString';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import dayjs from '@/lib/dayjs';
import { FormAutoselectOption } from '@/types/form';
import { UserConversation, UserFeedback } from '@/types/teamboard';

import { FeedbackFormValues } from './types';

export const convertFeedbackToFormInitialState = (
	{
		actionsTodo,
		conversationId,
		externalFeedbackDate,
		externalFeedbackType,
		feedback,
		projectId,
		projectName,
		reviewer,
	}: UserFeedback,
	conversationOptions: Array<FormAutoselectOption>
): FeedbackFormValues => ({
	actions: actionsTodo,
	conversation: conversationId
		? conversationOptions.find(({ id }) => id === conversationId) || null
		: null,
	date: dayjs(externalFeedbackDate),
	feedback,
	project: {
		id: projectId,
		name: projectName,
	},
	reviewer,
	type: {
		id: externalFeedbackType,
		name: externalFeedbackType,
	},
});

export const convertConversationResponseToOptions = (
	rawConversations: Array<UserConversation>
): Array<FormAutoselectOption> =>
	rawConversations.map(({ id, supervisorFullName, date }) => ({
		id,
		name: `${formatDateString(date)} - ${supervisorFullName}`,
	}));

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.m,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		hasContols: true,
		windowHeight,
	}),
});
