import { ContractWidget } from '../ContractWidget';
import { EventsWidget } from '../EventsWidget';
import { InterviewsWidget } from '../InterviewsWidget';
import { KPIWidget } from '../KPIWidget';
import { PerformanceReviewWidget } from '../PerformanceReviewWidget';
import { ProbationWidget } from '../ProbationWidget';
import { RequestsWidget } from '../RequestsWidget';
import { TodayAbsencesWidget } from '../TodayAbsencesWidget';

import { DashboardContentContainer, DashboardContentWrapper } from './styles';

export function DashboardContent() {
	return (
		<DashboardContentContainer>
			<DashboardContentWrapper>
				<InterviewsWidget />
				<RequestsWidget />
				<ContractWidget />
				<EventsWidget />
				<PerformanceReviewWidget />
				<TodayAbsencesWidget />
				<ProbationWidget />
				<KPIWidget />
			</DashboardContentWrapper>
		</DashboardContentContainer>
	);
}
