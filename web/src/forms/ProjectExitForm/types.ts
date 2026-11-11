import { Theme } from '@emotion/react';

import { Dayjs } from '@/lib/dayjs';
import {
	FormAutoselectOption,
	FormAutoselectOptionWithCustomValue,
} from '@/types/forms';
import { ProjectExitEntity } from '@/types/projectExits';

export interface ProjectExitFormProps {
	onClose: VoidFunction;
	projectExit?: ProjectExitEntity;
}

export interface GetPopUpStylesConfigProps {
	theme: Theme;
	windowHeight: number;
	windowWidth: number;
}

export interface ProjectExitFormValues {
	user: FormAutoselectOption | null;
	client: FormAutoselectOption | null;
	project: FormAutoselectOption | null;
	closingDate: Dayjs | null;
	closingReason: FormAutoselectOptionWithCustomValue | null;
}

export type ValidatedProjectExitFormValues = {
	[Key in keyof ProjectExitFormValues]: NonNullable<ProjectExitFormValues[Key]>;
};
