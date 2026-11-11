import { Dayjs } from 'dayjs';

import { Inquiry } from '@/queries/inquiries/types';
import { UserAbsenceItem } from '@/types/absences';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import { Document, DocumentResponse } from '@/types/document';

export interface InitialDataValues {
	absenceName: SimpleConfigurableConfigurationItem<string | number> | null;
	files: Document[];
	notes: string | null;
	dateFrom: Dayjs | null;
	dateTo: Dayjs | null;
	hours: string | null;
	documentsIds?: DocumentResponse[] | null;
}

export type RequestDataValues = InitialDataValues &
	Pick<UserAbsenceItem, 'agreementTemplateId' | 'inquiryTemplateId'>;

export type FileWithName = File & { fileName?: string };

export interface RequestAbsenceFormProps {
	handleClose: VoidFunction;
	handleSubmit: (value: RequestDataValues) => Promise<Inquiry>;
	initialAbsenceTypeId: string | number | undefined;
	userId: number | undefined;
	locationId: number | null;
	isRequestCreating?: boolean;
	formType?: string;
}
