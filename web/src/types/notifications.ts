import { EMPLOYEE_CONTRACT_EXPIRES_IN } from '@/constants/notifications';

import { AbsenceType } from './absences';
import { DevelopmentPlanStatus } from './developmentPlan';

export enum NotificationType {
	WorkplaceBookingCancelled = 'WorkplaceBookingCancelled',
	MeetingRoomBookingCancelled = 'MeetingRoomBookingCancelled',
	TimeTrackReportCreated = 'TimeTrackReportCreated',
	TimeTrackReportGenerating = 'TimeTrackReportGenerating',
	SameTimeTrackReportGenerating = 'SameTimeTrackReportGenerating',
	TimeTrackReportGenerationFailed = 'TimeTrackReportGenerationFailed',
	DevelopmentPlanReviewed = 'DevelopmentPlanReviewed',
	DevelopmentPlanSentForReview = 'DevelopmentPlanSentForReview',
	EventRecordsRemoved = 'EventRecordsRemoved',
	UserInquiryCreated = 'UserInquiryCreated',
	UserInquiryCommentCreated = 'UserInquiryCommentCreated',
	UserInquiryApproved = 'UserInquiryApproved',
	UserInquiryRejected = 'UserInquiryRejected',
	EmployeeContractExpiring = 'EmployeeContractExpiring',
	InquiryTemplateApproversUpdated = 'InquiryTemplateApproversUpdated',
}

export enum DocumentFileType {
	Word = 'Word',
	Excel = 'Excel',
	Pdf = 'Pdf',
	Archive = 'Archive',
}

interface BaseNotificationFields {
	id: number;
	userId: number;
	sent: boolean;
	read: boolean;
	isDeleted: boolean;
	notificationType: NotificationType;
}

type ParsedNotification = BaseNotificationFields;

export interface RawUserNotification extends BaseNotificationFields {
	outputJson: string;
}

export interface TimeTrackReportCreatedNotification
	extends BaseNotificationFields {
	notificationType: NotificationType.TimeTrackReportCreated;
	ReportName: string;
	ReportFileId: string;
	DocumentType: DocumentFileType;
	SizeInKilobytes: number;
}

export interface WorkplaceBookingCancelledNotification
	extends ParsedNotification {
	notificationType: NotificationType.WorkplaceBookingCancelled;
	DateList: string[];
	WorkplaceNumber: number;
	WorkplaceId: number;
	WorkspaceId: number;
	WorkplaceOfficeId: number;
	BookingEntryId: number;
	IsBlock: boolean;
	Message: string;
}

export interface MeetingRoomBookingCancelledNotification
	extends ParsedNotification {
	notificationType: NotificationType.MeetingRoomBookingCancelled;
	DateList: string[];
	MeetingRoomId: number;
	MeetingRoomName: string;
	MeetingRoomEntryId: number;
	FromTime: string;
	ToTime: string;
	Message: string;
}
interface TimeTrackReportGeneratingNotification extends ParsedNotification {
	notificationType: NotificationType.TimeTrackReportGenerating;
	ReportName: string;
	DocumentType: DocumentFileType;
}

interface TimeTrackReportFailureNotification extends ParsedNotification {
	notificationType: NotificationType.TimeTrackReportGenerationFailed;
	ReportName: string;
	DocumentType: DocumentFileType;
}

interface DevelopmentPlanNotification extends ParsedNotification {
	Message: string;
	DevelopmentPlanId: number;
	UserId: number;
}

interface UserRequestNotificationItem extends ParsedNotification {
	Message: string;
	InquiryId: number;
	InquiryType: AbsenceType;
	UserId: number;
}

export interface UserRequestNotification extends UserRequestNotificationItem {
	notificationType: NotificationType.UserInquiryCreated;
}

export interface UserRequestApprovedOrRejectedNotification
	extends UserRequestNotificationItem {
	notificationType:
		| NotificationType.UserInquiryApproved
		| NotificationType.UserInquiryRejected;
}

export interface DevelopmentPlanReviewedNotification
	extends DevelopmentPlanNotification {
	notificationType: NotificationType.DevelopmentPlanReviewed;
	Status: DevelopmentPlanStatus;
}

export interface DevelopmentPlanSentForReviewNotification
	extends DevelopmentPlanNotification {
	notificationType: NotificationType.DevelopmentPlanSentForReview;
}

interface EventRecordsRemovedOutputJson {
	DevelopmentPlanId: number;
}

export interface EventRecordsRemovedNotification
	extends ParsedNotification,
		EventRecordsRemovedOutputJson {
	notificationType: NotificationType.EventRecordsRemoved;
}

interface UserRequestCommentNotificationItem extends ParsedNotification {
	Message: string;
	InquiryId: number;
	InquiryCommentId: number;
	UserId: number;
}

export interface UserRequestCommentNotification
	extends UserRequestCommentNotificationItem {}

export interface InquiryApproversUpdatedNotification
	extends ParsedNotification {
	Message: string;
	InquiryId: number;
	InquiryType: AbsenceType;
}

export interface EmployeeContractExpiringNotification
	extends ParsedNotification {
	notificationType: NotificationType.EmployeeContractExpiring;
	UserId: number;
	UserFirstName: string;
	UserLastName: string;
	DaysLeft: keyof typeof EMPLOYEE_CONTRACT_EXPIRES_IN;
}

export type UserNotification =
	| TimeTrackReportCreatedNotification
	| WorkplaceBookingCancelledNotification
	| MeetingRoomBookingCancelledNotification
	| TimeTrackReportGeneratingNotification
	| TimeTrackReportFailureNotification
	| DevelopmentPlanReviewedNotification
	| DevelopmentPlanSentForReviewNotification
	| EventRecordsRemovedNotification
	| UserRequestNotification
	| UserRequestCommentNotification
	| EmployeeContractExpiringNotification
	| InquiryApproversUpdatedNotification;
