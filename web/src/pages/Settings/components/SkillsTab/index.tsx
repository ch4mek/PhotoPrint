import {
	LoaderProvider,
	useLoaderContext,
} from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useState } from 'react';

import { Loader } from '@/components/Loader';
import { DepartmentType } from '@/types/configuration';
import { Specialization } from '@/types/settings';

import { DepartmentsList } from './components/DepartmentsList';
import { HardSkills } from './components/HardSkills';
import { SoftSkills } from './components/SoftSkills';
import { SpecializationsList } from './components/SpecializationsList';
import {
	LeftListContainer,
	RightListContainer,
	SkillsListsWrapper,
} from './styles';

export function SkillsTabInner() {
	const [selectedDepartmentId, setSelectedDepartmentId] = useState<
		string | null
	>(null);

	const { isLoading } = useLoaderContext();

	const [selectedSpecialization, setSelectedSpecialization] =
		useState<Specialization | null>(null);

	const handleDepartmentItemClick = useCallback(
		(department: DepartmentType) => {
			setSelectedDepartmentId((prevDepartment) =>
				prevDepartment === department.id ? null : department.id
			);

			if (!selectedDepartmentId) {
				setSelectedSpecialization(null);
			}
		},
		[]
	);

	const handleSpecializationItemClick = useCallback(
		(specialization: Specialization) => {
			if (selectedDepartmentId) {
				setSelectedSpecialization((prevspecialization) =>
					prevspecialization?.id === specialization.id ? null : specialization
				);
			}
		},
		[selectedDepartmentId]
	);

	return (
		<Loader loaderType="soft" isLoading={isLoading}>
			<SkillsListsWrapper>
				<LeftListContainer>
					<DepartmentsList
						handleDepartmentItemClick={handleDepartmentItemClick}
						selectedDepartmentId={selectedDepartmentId}
					/>
					<SpecializationsList
						{...{
							selectedDepartmentId,
							handleSpecializationItemClick,
							selectedSpecialization,
						}}
					/>
					<HardSkills
						{...{
							selectedDepartmentId,
							selectedSpecialization,
						}}
					/>
				</LeftListContainer>
				<RightListContainer>
					<SoftSkills />
				</RightListContainer>
			</SkillsListsWrapper>
		</Loader>
	);
}

export function SkillsTab() {
	return (
		<LoaderProvider>
			<SkillsTabInner />
		</LoaderProvider>
	);
}
