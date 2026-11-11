import { Theme } from '@emotion/react';

import { feedbackAutoselectOptions } from '@/constants/feedback';
import { popUpSizes } from '@/constants/popUp';
import {
	CONVERSATION_TYPE_OPTIONS,
	PASSED_STATUS_OPTIONS,
} from '@/forms/TeamboardForm/Conversation/constants';
import { findById } from '@/helpers/findById';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import dayjs from '@/lib/dayjs';
import { FormAutoselectOption } from '@/types/form';
import { UserConversation } from '@/types/teamboard';

import { ConversationFormValues, StatusType } from './types';

export const convertConversationToFormInitialState = ({
	agreements,
	date,
	employeeTheses,
	theses,
	conversationResult,
	conversationType,
	performanceReviewDate,
	status,
	odpConversationDocumentLinks,
	odpConversationFiles,
}: UserConversation): ConversationFormValues => ({
	agreements,
	date: date ? dayjs(date).startOf('day') : null,
	employeeTheses,
	theses,
	conversationResult:
		findById(feedbackAutoselectOptions, conversationResult) || null,
	conversationType:
		findById(CONVERSATION_TYPE_OPTIONS, conversationType) ||
		CONVERSATION_TYPE_OPTIONS[0],
	performanceReviewDate: performanceReviewDate
		? dayjs(performanceReviewDate).startOf('day')
		: null,
	passed:
		(status &&
			findById<FormAutoselectOption>(
				PASSED_STATUS_OPTIONS,
				Number(status === StatusType.Passed)
			)) ||
		null,
	files: odpConversationFiles || [],
	documentLinks: odpConversationDocumentLinks.map(({ link }) => link) || [],
});

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.m,
	},
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowHeight,
		hasContols: true,
	}),
});
