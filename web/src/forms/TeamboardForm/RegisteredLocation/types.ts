import { CalendaryCountry, LocationType } from '@/types/configuration';

export interface RegisteredLocationFormProps {
	handleClose: () => void;
	onSubmit: (values: UpdateRegisteredLocation) => Promise<object | undefined>;
	id: number;
	initialValues: RegisteredLocationFormValues;
}

export interface RegisteredLocationFormValues {
	currentLocation: string | null;
	registeredLocation: CalendaryCountry | null;
	location: LocationType | null;
}

export interface UpdateRegisteredLocation {
	currentLocation: string | null;
	registeredLocation: number;
	location: string;
	userId: number;
}
