export type MessageStripStatus = 'warning' | 'error' | 'success' | 'info';
export type MessageStripProps = {
    status?: MessageStripStatus;
    text: string;
    isFullWidth?: boolean;
    dataTestId?: string;
};
export type StatusProps = {
    status: MessageStripStatus;
    isFullWidth?: boolean;
};
