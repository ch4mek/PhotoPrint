import { Loader } from '@/components/Loader';
import { currentDate, DateFormat } from '@/constants/date';
import {
	DEFAULT_PLANS_ORDER_BY_FIELD,
	developmentPlanSectionDefaultStatusFilters,
	developmentPlanSectionQueryParams,
} from '@/constants/developmentPlan';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';
import { AbsencesWidget } from '@/pages/Home/HomeContent/components/AbsencesWidget';
import { useGetUsersByBirthdayDate } from '@/queries/home/useGetUsersByBirthdayDate';
import { useGetUserUpcomingEvents } from '@/queries/home/useGetUserUpcomingEvents';
import { useGetTimesheetCalendar } from '@/queries/timesheet/useGetTimesheetCalendar';
import { useUserDevelopmentPlans } from '@/queries/useUserDevelopmentPlans';

import { BirthdaysWidget } from './components/BirthdaysWidget';
import { CompanyLinksWidget } from './components/CompanyLinksWidget';
import { DevelopmentPlanWidget } from './components/DevelopmentPlanWidget';
import { HomeHeader } from './components/HomeHeader';
import { LeaderboardWidget } from './components/LeaderboardWidget';
import { ProgrammerDayGame } from './components/ProgrammerDayGame';
import { ProgrammerDayHeader } from './components/ProgrammerDayHeader';
import { TimesheetWidget } from './components/TimesheetWidget';
import { UpcomingEventsWidget } from './components/UpcomingEventsWidget';
import {
	homeWidgetsAccessRoles,
	IS_PROGRAMMER_DAY_GAME_ENABLED,
} from './constants';
import {
	HomeContentWrapper,
	HomePageWrapper,
	ProgrammerDayContentWrapper,
	ProgrammerDayLeaderboardWrapper,
	ProgrammerDaySectionWrapper,
} from './styles';

const {
	developmentPlan,
	upcomingEvent,
	timesheet,
	birthdays,
	absences,
	companyLinks,
} = homeWidgetsAccessRoles;

export function HomeContent() {
	const { role, id } = useAuth();

	const isDevPlanWidgetEnable = developmentPlan.includes(role);
	const isUpcomingEventsWidgetEnable = upcomingEvent.includes(role);
	const isCompanyLinksWidgetEnable = companyLinks.includes(role);
	const isBirthdaysWidgetEnable = birthdays.includes(role);
	const isTimesheetWidgetEnable = timesheet.includes(role);
	const isAbsencesWidgetEnable = absences.includes(role);

	const {
		data: developmentPlans,
		isLoading: isDevelopmentPlanLoading,
		isFetching: isDevelopmentPlanFetching,
	} = useUserDevelopmentPlans(
		{
			userId: id,
			filters: {
				status: developmentPlanSectionDefaultStatusFilters,
			},
			orderBy: getOrderValueForDevelopmentPlans(
				DEFAULT_PLANS_ORDER_BY_FIELD,
				SortDirection.ASC
			),
			...developmentPlanSectionQueryParams,
		},
		{
			enabled: isDevPlanWidgetEnable,
		}
	);

	const { data: upcomingEvents, isLoading: isUpcomingEventsLoading } =
		useGetUserUpcomingEvents(id, {
			enabled: isUpcomingEventsWidgetEnable && !!id,
		});

	const { data: timesheetData, isLoading: isTimesheetDataLoading } =
		useGetTimesheetCalendar(
			currentDate.startOf('day').format(DateFormat.YYYY_MM_DD),
			{
				enabled: isTimesheetWidgetEnable && !!id,
			}
		);

	const { data: birthdaysData, isLoading: isBirthdaysLoading } =
		useGetUsersByBirthdayDate(
			currentDate.startOf('day').format(DateFormat.YYYY_MM_DD),
			{
				enabled: isBirthdaysWidgetEnable && !!id,
			}
		);

	const isLoading =
		isDevelopmentPlanLoading || isUpcomingEventsLoading || isBirthdaysLoading;

	return (
		<HomePageWrapper>
			<Loader {...{ isLoading }} size="medium">
				{IS_PROGRAMMER_DAY_GAME_ENABLED ? (
					<ProgrammerDaySectionWrapper>
						<ProgrammerDayHeader />
						<ProgrammerDayContentWrapper>
							<ProgrammerDayGame />
							<ProgrammerDayLeaderboardWrapper>
								<LeaderboardWidget />
							</ProgrammerDayLeaderboardWrapper>
						</ProgrammerDayContentWrapper>
					</ProgrammerDaySectionWrapper>
				) : (
					<HomeHeader />
				)}
				<HomeContentWrapper>
					{isTimesheetWidgetEnable && (
						<TimesheetWidget
							data={timesheetData || []}
							isLoading={isTimesheetDataLoading}
						/>
					)}
					{isDevPlanWidgetEnable && (
						<DevelopmentPlanWidget
							data={developmentPlans?.items || []}
							isLoading={isDevelopmentPlanFetching}
						/>
					)}
					{isUpcomingEventsWidgetEnable && (
						<UpcomingEventsWidget data={upcomingEvents || []} />
					)}
					{isBirthdaysWidgetEnable && (
						<BirthdaysWidget data={birthdaysData || []} />
					)}
					{isAbsencesWidgetEnable && <AbsencesWidget />}
					{isCompanyLinksWidgetEnable && <CompanyLinksWidget />}
				</HomeContentWrapper>
			</Loader>
		</HomePageWrapper>
	);
}
