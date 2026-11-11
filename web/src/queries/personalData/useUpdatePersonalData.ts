import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import {
	absencesQueriesKeys,
	BookingQueriesKeys,
	dashboardQueriesKeys,
	interviewQueriesKeys,
	payloadQueriesKeys,
	performanceReviewQueriesKeys,
	projectExitQueriesKeys,
	ratesQueriesKeys,
	TeamboardQueriesKeys,
	TimesheetQueryKeys,
	UsersQueriesKeys,
} from '@/constants/queriesKeys';
import {
	GET_PERSONAL_DATA_API_ENDPOINT,
	getUserByIdEndPoint,
} from '@/constants/queryPaths';
import { UpdatePersonalData } from '@/forms/TeamboardForm/PersonalData/types';
import { graduateJobPlacementToBool } from '@/helpers/formatGraduateJobPlacement';
import { getDateFormat } from '@/helpers/getDateFormat';
import { getStringOrFallback } from '@/helpers/getStringOrFallback';
import { PersonalDataResponse } from '@/types/personalData';

export const useUpdatePersonalData = (
	options: UseMutationOptions<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		UpdatePersonalData
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		UpdatePersonalData
	>({
		...options,
		onSuccess: (...args) => {
			const [, { userId }] = args;

			const queriesToInvalidate = [
				[TeamboardQueriesKeys.personalData, userId],
				[TeamboardQueriesKeys.teamboardUsersLookup],
				[UsersQueriesKeys.users],
				[UsersQueriesKeys.allUsers],
				[UsersQueriesKeys.usersTabLookup],
				[UsersQueriesKeys.usersTab],
				[UsersQueriesKeys.usersLookup],
				[performanceReviewQueriesKeys.getAllPerformanceReviews],
				[interviewQueriesKeys.getInterviewsData],
				[interviewQueriesKeys.getInterviewUsersLookup],
				[payloadQueriesKeys.getPayloadData],
				[dashboardQueriesKeys.getContractExpirationLookup],
				[dashboardQueriesKeys.getRequestsWidget],
				[dashboardQueriesKeys.getPerformanceReviewLookup],
				[dashboardQueriesKeys.getProbationLookup],
				[dashboardQueriesKeys.getTodaysInterviewsLookup],
				[dashboardQueriesKeys.getApprovedAbsences],
				[payloadQueriesKeys.getTotalPayloadData],
				[dashboardQueriesKeys.getEmployeeEvents],
				[getUserByIdEndPoint(userId)],
			];

			const delayedQueries = [
				[TimesheetQueryKeys.timesheetManager],
				[BookingQueriesKeys.bookings],
				[BookingQueriesKeys.meetingRoomsBookings],
				[BookingQueriesKeys.reservations],
				[BookingQueriesKeys.meetingRoomReservations],
				[dashboardQueriesKeys.getTimesheetUsersLookup],
				[dashboardQueriesKeys.getProbationLookup],
				[dashboardQueriesKeys.getApprovedAbsences],
				[absencesQueriesKeys.getInquiriesCalendar],
				[projectExitQueriesKeys.getProjectExits],
				[ratesQueriesKeys.getRates],
				[ratesQueriesKeys.getRatesUsers],
			];

			queriesToInvalidate.forEach((queryKey) =>
				queryClient.invalidateQueries({ queryKey })
			);

			// TODO: [WORKAROUND] Timeout is a temporary fix due to backend race condition —
			// server doesn't return updated data immediately after mutation
			// Should be removed once backend ensures consistent response timing
			setTimeout(() => {
				delayedQueries.forEach((queryKey) => {
					queryClient.invalidateQueries({ queryKey });
				});
			}, 500);

			options.onSuccess?.(...args);
		},
		mutationFn: ({
			firstName,
			lastName,
			firstNameRu,
			lastNameRu,
			patronymicRu,
			position,
			department,
			group,
			corporateEmail,
			personalEmail,
			personalEmailSecond,
			userId,
			dateOfBirth,
			telegram,
			englishLevel,
			personalPhone,
			militaryServiceStatus,
			graduateJobPlacement,
			linkedIn,
		}) =>
			axios
				.patch(GET_PERSONAL_DATA_API_ENDPOINT, {
					firstName,
					lastName,
					corporateEmail,
					personalPhone,
					userId,
					firstNameRu: getStringOrFallback(firstNameRu),
					lastNameRu: getStringOrFallback(lastNameRu),
					patronymicRu: getStringOrFallback(patronymicRu),
					personalEmail: getStringOrFallback(personalEmail),
					personalEmailSecond: getStringOrFallback(personalEmailSecond),
					dateOfBirth: getDateFormat(dateOfBirth, YYYY_MM_DD_DAYJS_DATE_FORMAT),
					telegram: getStringOrFallback(telegram),
					position: position?.id,
					departmentId: department?.id,
					englishLevel: englishLevel?.id,
					groupId: group?.id,
					linkedIn: getStringOrFallback(linkedIn),
					militaryServiceStatus,
					isGraduatedJobPlacement:
						graduateJobPlacementToBool(graduateJobPlacement),
				})
				.then(({ data }) => data)
				.catch((error) => {
					throw error;
				}),
	});
};
