import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { getFullName } from '@/helpers/utils';
import dayjs from '@/lib/dayjs';
import { getInterviewStatusLabel } from '@/pages/Staffing/components/InterviewTab/utils/getInterviewStatusLabel';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import {
	CreateUpdateInterviewTemplate,
	InterviewData,
	LookupSaleManagersResponse,
} from '@/types/interview';

import { initialFormValues } from './constants';
import { GetInitialValuesParams, InterviewFormValues } from './types';

export const getFormTitle = (isEditing: boolean) =>
	`Interview ${isEditing ? 'editing' : 'adding'}`;

export const getUserOptions = (
	users?: LookupSaleManagersResponse[]
): SimpleConfigurableConfigurationItem<number>[] =>
	(users || []).map(({ id, firstName, lastName, firstNameRu, lastNameRu }) => ({
		id,
		name: getFullName(firstName, lastName, firstNameRu, lastNameRu),
	}));

const getSaleManager = (
	interview: InterviewData,
	isSaleManager: boolean,
	saleManagerOption: SimpleConfigurableConfigurationItem<number>
) => {
	if (interview.salesResponsibleUserId) {
		return {
			id: interview.salesResponsibleUserId,
			name: getFullName(
				interview.salesResponsibleUserFirstName!,
				interview.salesResponsibleUserLastName!,
				interview.salesResponsibleUserFirstNameRu,
				interview.salesResponsibleUserLastNameRu
			),
		};
	}

	if (isSaleManager) {
		return saleManagerOption;
	}

	return null;
};

export const getInitialValues = ({
	isSaleManager,
	saleManagerOption,
	interview,
	parentInterview,
	preselectedUser,
}: GetInitialValuesParams): InterviewFormValues => {
	if (!interview) {
		if (parentInterview) {
			return {
				...initialFormValues,
				user: {
					id: parentInterview.userId,
					name: getFullName(
						parentInterview.userFirstName,
						parentInterview.userLastName,
						parentInterview.userFirstNameRu,
						parentInterview.userLastNameRu
					),
				},
				specialization: parentInterview.specialization || '',
				client: parentInterview.clientName || '',
				jiraTask: parentInterview.jiraTask || '',
				type: null,
				saleManager: getSaleManager(
					parentInterview,
					isSaleManager,
					saleManagerOption
				),
			};
		}

		return {
			...initialFormValues,
			user: preselectedUser ?? null,
			saleManager: isSaleManager ? saleManagerOption : null,
		};
	}

	const interviewDate = interview.userInterviewDate
		? dayjs(interview.userInterviewDate)
		: null;

	return {
		user: {
			id: interview.userId,
			name: getFullName(
				interview.userFirstName,
				interview.userLastName,
				interview.userFirstNameRu,
				interview.userLastNameRu
			),
		},
		specialization: interview.specialization || '',
		client: interview.clientName || '',
		jiraTask: interview.jiraTask || '',
		type: interview.interviewType
			? {
					id: interview.interviewType,
					name: interview.interviewType,
			  }
			: null,
		status: interview.status
			? {
					id: interview.status,
					name: getInterviewStatusLabel(interview.status),
			  }
			: null,
		date: interviewDate,
		time: interview.userInterviewFromUtc?.slice(0, 5) || '',
		saleManager: getSaleManager(interview, isSaleManager, saleManagerOption),
		reasonForRefusal: interview.reasonForRefusal || '',
		note: interview.notes || '',
	};
};

export const convertFormValues = (
	values: InterviewFormValues
): CreateUpdateInterviewTemplate => {
	const { date, time } = values;
	const normalizedDate = date ? dayjs(date).utc(true) : null;

	return {
		userId: values.user!.id,
		specialization: values.specialization,
		client: values.client || null,
		jiraTask: values.jiraTask || null,
		type: values.type?.id || null,
		status: values.status?.id || null,
		interviewDate: normalizedDate?.format(YYYY_MM_DD_DAYJS_DATE_FORMAT) || null,
		interviewTimeFromUtc: time ? `${time}:00` : null,
		saleManagerId: values.saleManager?.id || null,
		reasonForRefusal: values.reasonForRefusal || null,
		notes: values.note || null,
	};
};
