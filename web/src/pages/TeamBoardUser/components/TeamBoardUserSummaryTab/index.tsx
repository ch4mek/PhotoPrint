import {
	LoaderProvider,
	useLoaderContext,
} from '@mms/mms-ui-library/components/ManagedLoader';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useContext } from 'react';

import { isFullAccess, isLimitedHRAccess } from '../../utils';
import { EmploymentHistoryContainer } from '../EmploymentHistory';
import { LocationContainer } from '../LocationSection';
import { PersonalDataContainer } from '../PersonalData';
import { GRID_AREA_NAMES } from '../SummaryTabLayout/constants';
import { Column, Wrapper } from '../SummaryTabLayout/styles';

import { Loader } from '@/components/Loader';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { TeamboardTableMode } from '@/types/teamboard';

import { AbsenceSection } from './AbsenceSection';
import { ConversationsSection } from './ConverstationSection';
import { DevelopmentPlanSection } from './DevelopmentPlanSection';
import { FeedbackSection } from './FeedbackSection';
import { HardSkillSection } from './HardSkillSection';
import { PlanContainer } from './PlanSection';
import { ProjectSection } from './ProjectSection';
import { SoftSkillSection } from './SoftSkillSection';

function TeamBoardUserSummaryTabInner() {
	const { accessLevel } = useContext(PersonalDataContext);

	const { isLoading } = useLoaderContext();
	const isDesktop = useHasScreenWidth(breakpoints.desktopXS);

	const hasFullAccess = isFullAccess(accessLevel);
	const hasHRAccess = isLimitedHRAccess(accessLevel);

	const shouldShowConversations = hasFullAccess || hasHRAccess;

	return (
		<Loader isLoading={isLoading} loaderType="soft" size="small">
			<Wrapper>
				<Column orderName={GRID_AREA_NAMES.firstGroup}>
					<PersonalDataContainer />
					<LocationContainer />
					{isDesktop && <EmploymentHistoryContainer />}
					{hasFullAccess && <AbsenceSection />}
				</Column>

				<Column orderName={GRID_AREA_NAMES.secondGroup}>
					{hasFullAccess && (
						<>
							<PlanContainer />
							<ProjectSection />
							<HardSkillSection />
							<SoftSkillSection />
						</>
					)}
					{!hasFullAccess && <AbsenceSection />}
					{shouldShowConversations && <ConversationsSection />}
					{hasFullAccess && (
						<>
							<FeedbackSection />
							<DevelopmentPlanSection mode={TeamboardTableMode.Section} />
						</>
					)}
					{!isDesktop && <EmploymentHistoryContainer />}
				</Column>
			</Wrapper>
		</Loader>
	);
}

export function TeamBoardUserSummaryTab() {
	return (
		<LoaderProvider>
			<TeamBoardUserSummaryTabInner />
		</LoaderProvider>
	);
}
