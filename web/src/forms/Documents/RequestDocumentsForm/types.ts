import { Inquiry } from '@/queries/inquiries/types';
import { Document, DocumentResponse } from '@/types/document';

export interface InitialDataValues {
	documentType: { id: string | number; name: string } | null;
	files: Document[];
	notes: string | null;
}

export type RequestDocumentsValues = InitialDataValues & {
	documentsIds?: DocumentResponse[] | null;
	inquiryTemplateId: number;
	agreementTemplateId?: number | null;
};

export interface RequestDocumentsFormProps {
	handleClose: VoidFunction;
	handleSubmit: (value: RequestDocumentsValues) => Promise<Inquiry>;
	userId: number | undefined;
	isRequestCreating?: boolean;
}

export type FileWithName = File & { fileName?: string };
