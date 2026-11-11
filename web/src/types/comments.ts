import { UserInquiryFiles } from './absences';

export interface CommentUser {
	id: number;
	firstName: string;
	lastName: string;
	email: string;
	departmentId: string;
	registeredLocationId: number | null;
	avatarFieldId: string | null;
	color: string | null;
	position: string | null;
}

export interface Comment {
	id: string;
	comment: string;
	createdByUser: CommentUser;
	createdAtUtc: string;
	updatedByUser: CommentUser | null;
	updatedAtUtc: string | null;
	files: UserInquiryFiles[];
}

interface CommentFileInput {
	fileId: string;
	fileName: string;
	fileSize: number;
}

export interface CommentAddData {
	comment: string;
	files: CommentFileInput[];
	notifyApprovers: boolean;
}
