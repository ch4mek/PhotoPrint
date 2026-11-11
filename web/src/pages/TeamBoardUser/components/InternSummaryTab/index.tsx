import {
	LoaderProvider,
	useLoaderContext,
} from '@mms/mms-ui-library/components/ManagedLoader';

import { EmploymentHistoryContainer } from '../EmploymentHistory';
import { InternshipSection } from '../InternshipSection';
import { internshipSectionRequiredClaims } from '../InternshipSection/constants';
import { LocationContainer } from '../LocationSection';
import { PersonalDataContainer } from '../PersonalData';
import { GRID_AREA_NAMES } from '../SummaryTabLayout/constants';
import { Column, Wrapper } from '../SummaryTabLayout/styles';
import { ConversationsSection } from '../TeamBoardUserSummaryTab/ConverstationSection';

import { Loader } from '@/components/Loader';
import { useAuth } from '@/context/AuthContext';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';

function InternSummaryTabInner() {
	const { isLoading } = useLoaderContext();
	const { claims } = useAuth();

	const shouldShowInternship = checkIsHasClaims(
		internshipSectionRequiredClaims,
		claims
	);

	return (
		<Loader isLoading={isLoading} loaderType="soft" size="small">
			<Wrapper>
				<Column orderName={GRID_AREA_NAMES.firstGroup}>
					<PersonalDataContainer />
					<LocationContainer />
					<EmploymentHistoryContainer />
				</Column>
				<Column orderName={GRID_AREA_NAMES.secondGroup}>
					{shouldShowInternship && <InternshipSection />}
					<ConversationsSection />
				</Column>
			</Wrapper>
		</Loader>
	);
}

export function InternSummaryTab() {
	return (
		<LoaderProvider>
			<InternSummaryTabInner />
		</LoaderProvider>
	);
}
