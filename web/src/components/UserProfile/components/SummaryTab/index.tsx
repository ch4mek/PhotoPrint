import { useContext } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { Loader } from '@/components/Loader';
import {
	DEFAULT_PLANS_ORDER_BY_FIELD,
	developmentPlanSectionDefaultStatusFilters,
	developmentPlanSectionQueryParams,
} from '@/constants/developmentPlan';
import { SortDirection } from '@/constants/sort';
import { getOrderValueForDevelopmentPlans } from '@/helpers/getOrderValueForDevelopmentPlans';
import { useEmploymentHistory } from '@/queries/employmentHistory/useEmploymentHistory';
import { useGetAssignedUsersSoftSkills } from '@/queries/softSkills/useGetAssignedUsersSoftSkills';
import { useUserDevelopmentPlans } from '@/queries/useUserDevelopmentPlans';
import { useUserSpecializationLookup } from '@/queries/useUserHardSkillsLookup';
import { useUserProjectsLookup } from '@/queries/useUserProjectsLookup';

import { DevelopmentPlan } from './components/DevelopmentPlan';
import { EmploymentHistory } from './components/EmploymentHistory';
import { HardSkills } from './components/HardSkills';
import { PersonalData } from './components/PersonalData';
import { Projects } from './components/Projects';
import { SoftSkills } from './components/SoftSkills';
import { SummaryTabColumn, SummaryTabWrapper } from './styles';

export function SummaryTab() {
	const {
		isFetching: isPersonalDataFetching,
		isLoading: isPersonalDataLoading,
		userId,
	} = useContext(PersonalDataContext);
	const { data: employmentHistory, isLoading: isEmploymentHistoryLoading } =
		useEmploymentHistory(userId);
	const { data: projects, isLoading: isProjectsLoading } =
		useUserProjectsLookup(userId);
	const { data: hardSkills, isLoading: isHardSkillsLoading } =
		useUserSpecializationLookup(userId, undefined);
	const { data: softSkills, isLoading: isSoftSkillsLoading } =
		useGetAssignedUsersSoftSkills(userId, undefined);

	const { data: developmentPlan, isLoading: isDevelopmentPlanLoading } =
		useUserDevelopmentPlans({
			userId,
			filters: {
				status: developmentPlanSectionDefaultStatusFilters,
			},
			orderBy: getOrderValueForDevelopmentPlans(
				DEFAULT_PLANS_ORDER_BY_FIELD,
				SortDirection.ASC
			),
			...developmentPlanSectionQueryParams,
		});

	const isLoading =
		isPersonalDataFetching ||
		isPersonalDataLoading ||
		isEmploymentHistoryLoading ||
		isProjectsLoading ||
		isHardSkillsLoading ||
		isSoftSkillsLoading ||
		isDevelopmentPlanLoading;

	return (
		<Loader isLoading={isLoading} size="small">
			<SummaryTabWrapper>
				<SummaryTabColumn>
					<PersonalData />
					<EmploymentHistory employmentHistory={employmentHistory} />
				</SummaryTabColumn>
				<SummaryTabColumn>
					<Projects projects={projects} />
					<HardSkills hardSkills={hardSkills} />
					<SoftSkills softSkills={softSkills} />
					<DevelopmentPlan developmentPlan={developmentPlan} />
				</SummaryTabColumn>
			</SummaryTabWrapper>
		</Loader>
	);
}
