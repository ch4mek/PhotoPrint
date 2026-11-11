import { Specialization } from '@/types/settings';

export interface SpecializationsListProps {
	selectedDepartmentId: string | null;
	selectedSpecialization: Specialization | null;
	handleSpecializationItemClick: (specialization: Specialization) => void;
}
