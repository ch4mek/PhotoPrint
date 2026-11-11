import { FromToProps } from '@/components/Grid';
import { ByPageResponse } from '@/queries/types';

export interface Certificate {
	certificateId: number;
	receiptDate: string;
	recipient: string;
	recipientId: string;
	technology: string;
	stack: string | null;
	createdAt: string;
	updatedAt: string;
	certificateImageURL?: string;
	certificateViewLink?: string;
}

export type CertificateResponse = ByPageResponse<Certificate>;

export interface CertificateFilters {
	certificateId: string;
	stack: string;
	receiptDate: FromToProps<Date>;
}
