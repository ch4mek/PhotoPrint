import { UpdateUserLocationPayload } from '@/queries/user/types';
import { LocationType } from '@/types/configuration';

export interface PersonalDataFormValues {
	location: LocationType | null;
}

export interface PersonalDataFormProps {
	handleClose: () => void;
	onSubmit: (payload: UpdateUserLocationPayload) => Promise<unknown>;
	userId: number;
	initialLocation: string;
	locationTypes: LocationType[];
}
