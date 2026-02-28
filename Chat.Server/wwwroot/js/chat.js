// =============================================
// Chat.js - SignalR Web Chat Client
// Features: Messenger-style bubbles, emoji,
//           file upload/download, image preview
// =============================================

// ====== Emoji Data ======
const emojiData = {
    smileys: ['😀', '😃', '😄', '😁', '😆', '😅', '🤣', '😂', '🙂', '😊', '😇', '🥰', '😍', '🤩', '😘', '😗', '😚', '😙', '🥲', '😋', '😛', '😜', '🤪', '😝', '🤑', '🤗', '🤭', '🤫', '🤔', '🫡', '🤐', '🤨', '😐', '😑', '😶', '🫥', '😏', '😒', '🙄', '😬', '🤥', '😌', '😔', '😪', '🤤', '😴', '😷', '🤒', '🤕', '🤧', '🥵', '🥶', '🥴', '😵', '🤯', '🤠', '🥳', '🥸', '😎', '🤓', '🧐', '😤', '😠', '😡', '🤬', '😈', '👿', '💀', '☠️', '💩', '🤡', '👹', '👺', '👻', '👽', '👾', '🤖'],
    gestures: ['👋', '🤚', '🖐️', '✋', '🖖', '🫱', '🫲', '🫳', '🫴', '👌', '🤌', '🤏', '✌️', '🤞', '🫰', '🤟', '🤘', '🤙', '👈', '👉', '👆', '🖕', '👇', '☝️', '🫵', '👍', '👎', '✊', '👊', '🤛', '🤜', '👏', '🙌', '🫶', '👐', '🤲', '🤝', '🙏', '✍️', '💅', '🤳', '💪', '🦾', '🦿', '🦵', '🦶', '👂', '🦻', '👃', '🧠', '🫀', '🫁', '🦷', '🦴', '👀', '👁️', '👅', '👄'],
    hearts: ['❤️', '🧡', '💛', '💚', '💙', '💜', '🖤', '🤍', '🤎', '💔', '❤️‍🔥', '❤️‍🩹', '❣️', '💕', '💞', '💓', '💗', '💖', '💘', '💝', '💟', '♥️', '💋', '💌', '💐', '🌹', '🌷', '🌺', '🌸', '💮', '🏵️', '🎀', '🎁'],
    animals: ['🐱', '🐶', '🐭', '🐹', '🐰', '🦊', '🐻', '🐼', '🐻‍❄️', '🐨', '🐯', '🦁', '🐮', '🐷', '🐸', '🐵', '🙈', '🙉', '🙊', '🐒', '🐔', '🐧', '🐦', '🐤', '🐣', '🐥', '🦆', '🦅', '🦉', '🦇', '🐺', '🐗', '🐴', '🦄', '🐝', '🪱', '🐛', '🦋', '🐌', '🐞', '🐜', '🪰', '🪲', '🪳', '🦟', '🦗', '🕷️', '🦂', '🐢', '🐍', '🦎', '🦖', '🦕', '🐙', '🦑', '🦐', '🦞', '🦀', '🪼', '🐡', '🐠', '🐟', '🐬', '🐳', '🐋', '🦈', '🐊', '🐅', '🐆', '🦓', '🦍', '🦧', '🐘', '🦛', '🦏', '🐪', '🐫', '🦒', '🦘', '🦬', '🐃', '🐂', '🐄', '🐎', '🐖', '🐏', '🐑', '🦙', '🐐', '🦌', '🐕', '🐩', '🦮', '🐕‍🦺', '🐈', '🐈‍⬛', '🪶', '🐓', '🦃', '🦤', '🦚', '🦜', '🦢', '🦩', '🕊️', '🐇', '🦝', '🦨', '🦡', '🦫', '🦦', '🦥', '🐁', '🐀', '🐿️', '🦔', '🐾', '🐉', '🐲', '🌵', '🎄', '🌲', '🌳', '🌴', '🪵', '🌱', '🌿', '☘️', '🍀', '🎍', '🪴', '🎋', '🍃', '🍂', '🍁', '🍄', '🌾', '💐', '🌷', '🌹', '🥀', '🌺', '🌸', '🌼', '🌻'],
    food: ['🍕', '🍔', '🍟', '🌭', '🍿', '🧂', '🥓', '🥚', '🍳', '🧇', '🥞', '🧈', '🍞', '🥐', '🥖', '🫓', '🥨', '🥯', '🥝', '🍏', '🍎', '🍐', '🍊', '🍋', '🍌', '🍉', '🍇', '🍓', '🫐', '🍈', '🍒', '🍑', '🥭', '🍍', '🥥', '🥑', '🍆', '🥔', '🥕', '🌽', '🌶️', '🫑', '🥒', '🥬', '🥦', '🧄', '🧅', '🍄', '🥜', '🌰', '🫘', '🍫', '🍬', '🍭', '🍮', '🎂', '🍰', '🧁', '🥧', '🍦', '🍧', '🍨', '🍩', '🍪', '🍯', '🥛', '🍼', '🫖', '☕', '🍵', '🧃', '🥤', '🧋', '🍶', '🍺', '🍻', '🥂', '🍷', '🥃', '🍸', '🍹', '🧉', '🍾', '🧊', '🥄', '🍴', '🍽️', '🥣', '🥡', '🥢', '🧆'],
    objects: ['⚽', '🏀', '🏈', '⚾', '🥎', '🎾', '🏐', '🏉', '🥏', '🎱', '🪀', '🏓', '🏸', '🏒', '🏑', '🥍', '🏏', '🪃', '🥅', '⛳', '🪁', '🏹', '🎣', '🤿', '🥊', '🥋', '🎽', '🛹', '🛼', '🛷', '⛸️', '🥌', '🎿', '⛷️', '🏂', '🪂', '🏋️', '🤸', '🤺', '⛹️', '🤾', '🏌️', '🏇', '🧘', '🏄', '🏊', '🤽', '🚣', '🧗', '🚵', '🚴', '🏆', '🥇', '🥈', '🥉', '🏅', '🎖️', '🏵️', '🎗️', '🎫', '🎟️', '🎪', '🎭', '🎨', '🎬', '🎤', '🎧', '🎼', '🎹', '🥁', '🪘', '🎷', '🎺', '🪗', '🎸', '🪕', '🎻', '🎲', '♟️', '🎯', '🎳', '🎮', '🕹️', '🧩', '💻', '🖥️', '📱', '📡', '🔋', '💡', '🔦', '🕯️', '💰', '💳', '💎', '⚙️', '🔧', '🔨', '🛠️', '⚒️', '🔩', '📎', '📌', '✂️', '📏', '📐', '🖊️', '✏️', '📝']
};

let connection = null;
let pendingFiles = [];

// ====== Initialize SignalR ======
async function initSignalR() {
    const statusEl = document.getElementById('connectionStatus');

    connection = new signalR.HubConnectionBuilder()
        .withUrl('/chathub')
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .build();

    // ====== SignalR Event Handlers ======
    connection.on('ReceiveMessage', (msg) => {
        renderMessage(msg);
        scrollToBottom();
    });

    connection.on('UpdateUserList', (users) => {
        updateUserList(users);
    });

    connection.on('ErrorMessage', (error) => {
        alert(error);
    });

    connection.on('MessageReactionsUpdated', (messageId, reactions) => {
        updateReactions(messageId, reactions);
    });

    connection.on('LoadAllReactions', (allReactions) => {
        for (const [msgId, reactions] of Object.entries(allReactions)) {
            updateReactions(parseInt(msgId), reactions);
        }
    });

    connection.onreconnecting(() => {
        statusEl.textContent = 'Reconnecting...';
        statusEl.className = 'connection-status connecting';
    });

    connection.onreconnected(() => {
        statusEl.textContent = 'Connected';
        statusEl.className = 'connection-status connected';
    });

    connection.onclose(() => {
        statusEl.textContent = 'Disconnected';
        statusEl.className = 'connection-status disconnected';
    });

    try {
        await connection.start();
        statusEl.textContent = 'Connected';
        statusEl.className = 'connection-status connected';

        // Join chat with current user (auth already handled by cookie)
        await connection.invoke('JoinChat', currentUser, '');
    } catch (err) {
        statusEl.textContent = 'Failed to connect';
        statusEl.className = 'connection-status disconnected';
        console.error('SignalR connection error:', err);
    }
}

// ====== Render Message ======
function renderMessage(msg) {
    const container = document.getElementById('chatMessages');
    const isOutgoing = msg.user === currentUser;
    const isSystem = msg.is_System || false;
    const isPrivate = msg.is_Private || false;
    const isFile = msg.isFile || false;

    const row = document.createElement('div');
    row.className = `message-row ${isSystem ? 'system' : isOutgoing ? 'outgoing' : 'incoming'} ${isPrivate ? 'private' : ''}`;
    row.dataset.messageId = msg.id;

    let html = '';

    // Sender name (only for incoming non-system messages)
    if (!isSystem && !isOutgoing) {
        const privateBadge = isPrivate ? '<span style="color:#b388ff;">🔒 </span>' : '';
        html += `<span class="message-sender">${privateBadge}${escapeHtml(msg.user)}</span>`;
    }

    if (isOutgoing && isPrivate) {
        html += `<span class="message-sender">🔒 To: ${escapeHtml(msg.recipient || '')}</span>`;
    }

    // Message bubble
    html += '<div class="message-bubble">';

    if (isFile) {
        // File message
        const fileName = msg.fileName || 'file';
        const fileUrl = msg.fileUrl || '#';
        const fileSize = msg.fileSize || 0;
        const isImage = /\.(jpg|jpeg|png|gif|bmp|webp|svg)$/i.test(fileName);

        if (isImage) {
            html += `<img src="${escapeHtml(fileUrl)}" class="chat-image" onclick="openLightbox('${escapeHtml(fileUrl)}')" alt="${escapeHtml(fileName)}"/>`;
        }

        html += `<a class="file-attachment" href="/api/download/${encodeURIComponent(fileUrl.split('/').pop())}" download>
            <span class="file-icon">${getFileIcon(fileName)}</span>
            <div class="file-details">
                <div class="file-name">${escapeHtml(fileName)}</div>
                <div class="file-size">${formatFileSize(fileSize)}</div>
            </div>
            <span class="file-download-btn">⬇ Download</span>
        </a>`;
    } else {
        // Text content - process for inline images
        const content = msg.content || '';
        html += processMessageContent(content);
    }

    html += '</div>';

    // Timestamp
    if (!isSystem) {
        const time = msg.timestamp ? new Date(msg.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : '';
        html += `<span class="message-time">${time}</span>`;
    }

    // Reactions (for non-system messages)
    if (!isSystem && msg.id) {
        html += `<div class="message-reactions" id="reactions-${msg.id}">
            <button class="reaction-btn" onclick="react(${msg.id}, '👍')">👍 <span class="reaction-count" id="rc-${msg.id}-like">0</span></button>
            <button class="reaction-btn" onclick="react(${msg.id}, '❤️')">❤️ <span class="reaction-count" id="rc-${msg.id}-heart">0</span></button>
            <button class="reaction-btn" onclick="react(${msg.id}, '😂')">😂 <span class="reaction-count" id="rc-${msg.id}-laugh">0</span></button>
        </div>`;
    }

    row.innerHTML = html;
    container.appendChild(row);
}

function processMessageContent(content) {
    // Escape HTML first, then process
    let text = escapeHtml(content);
    // Convert URLs to clickable links
    text = text.replace(/(https?:\/\/[^\s<]+)/g, '<a href="$1" target="_blank" rel="noopener" style="color:#7ab4ff;text-decoration:underline;">$1</a>');
    // Newlines
    text = text.replace(/\n/g, '<br>');
    return text;
}

// ====== Reactions ======
async function react(messageId, reactionType) {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        await connection.invoke('ReactToMessage', messageId, currentUser, reactionType);
    }
}

function updateReactions(messageId, reactions) {
    const likeEl = document.getElementById(`rc-${messageId}-like`);
    const heartEl = document.getElementById(`rc-${messageId}-heart`);
    const laughEl = document.getElementById(`rc-${messageId}-laugh`);

    if (likeEl) likeEl.textContent = reactions['👍'] || 0;
    if (heartEl) heartEl.textContent = reactions['❤️'] || 0;
    if (laughEl) laughEl.textContent = reactions['😂'] || 0;
}

// ====== User List ======
function updateUserList(users) {
    const userListEl = document.getElementById('userList');
    const recipientEl = document.getElementById('recipientSelect');

    userListEl.innerHTML = '';
    users.forEach(user => {
        const li = document.createElement('li');
        li.textContent = user;
        if (user === currentUser) li.classList.add('current-user');
        userListEl.appendChild(li);
    });

    // Update recipient dropdown
    const currentRecipient = recipientEl.value;
    recipientEl.innerHTML = '<option value="">Everyone (Public)</option>';
    users.forEach(user => {
        if (user !== currentUser) {
            const option = document.createElement('option');
            option.value = user;
            option.textContent = user;
            if (user === currentRecipient) option.selected = true;
            recipientEl.appendChild(option);
        }
    });
}

// ====== Send Message ======
async function sendMessage() {
    const input = document.getElementById('messageInput');
    const message = input.value.trim();
    const recipient = document.getElementById('recipientSelect').value;

    if (!message && pendingFiles.length === 0) return;

    // Send pending files first
    for (const file of pendingFiles) {
        await uploadAndSendFile(file);
    }
    clearPendingFiles();

    // Send text message
    if (message) {
        if (recipient) {
            await connection.invoke('SendPrivateMessage', currentUser, recipient, message);
        } else {
            await connection.invoke('SendMessage', currentUser, message);
        }
    }

    input.value = '';
    input.style.height = 'auto';
}

// ====== File Upload ======
async function uploadAndSendFile(file) {
    const progressEl = document.getElementById('uploadProgress');
    const fileNameEl = document.getElementById('uploadFileName');
    const percentEl = document.getElementById('uploadPercent');
    const fillEl = document.getElementById('progressBarFill');

    progressEl.style.display = 'block';
    fileNameEl.textContent = `Uploading: ${file.name}`;
    percentEl.textContent = '0%';
    fillEl.style.width = '0%';

    try {
        const formData = new FormData();
        formData.append('file', file);

        const xhr = new XMLHttpRequest();

        await new Promise((resolve, reject) => {
            xhr.upload.addEventListener('progress', (e) => {
                if (e.lengthComputable) {
                    const pct = Math.round((e.loaded / e.total) * 100);
                    percentEl.textContent = `${pct}%`;
                    fillEl.style.width = `${pct}%`;
                }
            });

            xhr.addEventListener('load', () => {
                if (xhr.status === 200) {
                    resolve(JSON.parse(xhr.responseText));
                } else {
                    reject(new Error(`Upload failed: ${xhr.status}`));
                }
            });

            xhr.addEventListener('error', () => reject(new Error('Upload failed')));
            xhr.open('POST', '/api/upload');
            xhr.send(formData);
        }).then(async (result) => {
            // Send file message via SignalR
            const recipient = document.getElementById('recipientSelect').value;
            const fileMsg = `[FILE] ${result.fileName} (${formatFileSize(result.fileSize)})`;

            if (recipient) {
                await connection.invoke('SendPrivateMessage', currentUser, recipient, fileMsg);
            } else {
                await connection.invoke('SendMessage', currentUser, fileMsg);
            }

            // Also send a file-type message for rendering
            renderMessage({
                id: 0,
                user: currentUser,
                content: '',
                timestamp: new Date().toISOString(),
                is_System: false,
                is_Private: !!recipient,
                recipient: recipient || null,
                isFile: true,
                fileName: result.fileName,
                fileUrl: result.fileUrl,
                fileSize: result.fileSize
            });
            scrollToBottom();
        });
    } catch (err) {
        alert('File upload failed: ' + err.message);
    } finally {
        setTimeout(() => { progressEl.style.display = 'none'; }, 1000);
    }
}

// ====== Emoji Picker ======
function initEmojiPicker() {
    const emojiBtn = document.getElementById('emojiBtn');
    const emojiPicker = document.getElementById('emojiPicker');
    const emojiClose = document.getElementById('emojiClose');
    const emojiGrid = document.getElementById('emojiGrid');
    const emojiSearch = document.getElementById('emojiSearch');

    let currentCategory = 'smileys';
    renderEmojiGrid(currentCategory);

    emojiBtn.addEventListener('click', () => {
        emojiPicker.style.display = emojiPicker.style.display === 'none' ? 'flex' : 'none';
    });

    emojiClose.addEventListener('click', () => {
        emojiPicker.style.display = 'none';
    });

    // Category buttons
    document.querySelectorAll('.emoji-cat-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.emoji-cat-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            currentCategory = btn.dataset.cat;
            renderEmojiGrid(currentCategory);
        });
    });

    // Search
    emojiSearch.addEventListener('input', () => {
        const query = emojiSearch.value.toLowerCase();
        if (!query) {
            renderEmojiGrid(currentCategory);
            return;
        }
        // Show all matching emojis from all categories
        const all = Object.values(emojiData).flat();
        // Simple search: show all (since emojis don't have text names, just show all)
        renderEmojiGridItems(all);
    });

    // Click outside to close
    document.addEventListener('click', (e) => {
        if (!emojiPicker.contains(e.target) && e.target !== emojiBtn) {
            emojiPicker.style.display = 'none';
        }
    });
}

function renderEmojiGrid(category) {
    const emojis = emojiData[category] || [];
    renderEmojiGridItems(emojis);
}

function renderEmojiGridItems(emojis) {
    const grid = document.getElementById('emojiGrid');
    grid.innerHTML = '';
    emojis.forEach(emoji => {
        const btn = document.createElement('button');
        btn.className = 'emoji-item';
        btn.textContent = emoji;
        btn.addEventListener('click', () => {
            const input = document.getElementById('messageInput');
            const start = input.selectionStart;
            const end = input.selectionEnd;
            input.value = input.value.substring(0, start) + emoji + input.value.substring(end);
            input.focus();
            input.selectionStart = input.selectionEnd = start + emoji.length;
        });
        grid.appendChild(btn);
    });
}

// ====== File Attachment ======
function initFileAttachment() {
    const attachBtn = document.getElementById('attachBtn');
    const fileInput = document.getElementById('fileInput');

    attachBtn.addEventListener('click', () => fileInput.click());

    fileInput.addEventListener('change', () => {
        handleFileSelection(fileInput.files);
        fileInput.value = '';
    });

    // Drag and drop on the chat area
    const chatMain = document.querySelector('.chat-main');
    chatMain.addEventListener('dragover', (e) => {
        e.preventDefault();
        chatMain.style.outline = '2px dashed var(--primary)';
        chatMain.style.outlineOffset = '-4px';
    });

    chatMain.addEventListener('dragleave', () => {
        chatMain.style.outline = 'none';
    });

    chatMain.addEventListener('drop', (e) => {
        e.preventDefault();
        chatMain.style.outline = 'none';
        if (e.dataTransfer.files.length > 0) {
            handleFileSelection(e.dataTransfer.files);
        }
    });

    // Paste images
    document.getElementById('messageInput').addEventListener('paste', (e) => {
        const items = e.clipboardData?.items;
        if (!items) return;
        for (const item of items) {
            if (item.type.startsWith('image/')) {
                e.preventDefault();
                const file = item.getAsFile();
                if (file) handleFileSelection([file]);
            }
        }
    });
}

function handleFileSelection(files) {
    const previewBar = document.getElementById('imagePreviewBar');
    const previewContent = document.getElementById('imagePreviewContent');

    for (const file of files) {
        pendingFiles.push(file);

        if (file.type.startsWith('image/')) {
            // Show image preview
            const reader = new FileReader();
            reader.onload = (e) => {
                const item = document.createElement('div');
                item.className = 'preview-item';
                item.innerHTML = `
                    <img src="${e.target.result}" alt="${escapeHtml(file.name)}"/>
                    <button class="preview-remove" onclick="removePreviewItem(this, '${file.name}')">✕</button>
                `;
                previewContent.appendChild(item);
                previewBar.style.display = 'block';
            };
            reader.readAsDataURL(file);
        } else {
            // Show file name preview
            const item = document.createElement('div');
            item.className = 'preview-item';
            item.style.cssText = 'display:flex;align-items:center;gap:6px;padding:6px 10px;background:rgba(255,255,255,0.06);border-radius:8px;';
            item.innerHTML = `
                <span style="font-size:20px;">${getFileIcon(file.name)}</span>
                <span style="font-size:12px;color:var(--text-secondary);max-width:100px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">${escapeHtml(file.name)}</span>
                <button class="preview-remove" style="position:static;flex-shrink:0;" onclick="removePreviewItem(this, '${file.name}')">✕</button>
            `;
            previewContent.appendChild(item);
            previewBar.style.display = 'block';
        }
    }
}

function removePreviewItem(btn, fileName) {
    const item = btn.parentElement;
    item.remove();
    pendingFiles = pendingFiles.filter(f => f.name !== fileName);
    if (pendingFiles.length === 0) {
        document.getElementById('imagePreviewBar').style.display = 'none';
    }
}

function clearPendingFiles() {
    pendingFiles = [];
    document.getElementById('imagePreviewContent').innerHTML = '';
    document.getElementById('imagePreviewBar').style.display = 'none';
}

// ====== Lightbox ======
function openLightbox(url) {
    document.getElementById('lightboxImage').src = url;
    document.getElementById('lightboxModal').style.display = 'flex';
}

function closeLightbox() {
    document.getElementById('lightboxModal').style.display = 'none';
}

// ====== Utilities ======
function escapeHtml(str) {
    if (!str) return '';
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
}

function formatFileSize(bytes) {
    if (!bytes || bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function getFileIcon(fileName) {
    const ext = fileName.split('.').pop()?.toLowerCase() || '';
    const icons = {
        'pdf': '📄', 'doc': '📝', 'docx': '📝', 'txt': '📃',
        'xls': '📊', 'xlsx': '📊', 'csv': '📊',
        'ppt': '📽️', 'pptx': '📽️',
        'zip': '🗜️', 'rar': '🗜️', '7z': '🗜️', 'tar': '🗜️', 'gz': '🗜️',
        'mp3': '🎵', 'wav': '🎵', 'ogg': '🎵', 'flac': '🎵',
        'mp4': '🎬', 'avi': '🎬', 'mkv': '🎬', 'mov': '🎬', 'wmv': '🎬',
        'jpg': '🖼️', 'jpeg': '🖼️', 'png': '🖼️', 'gif': '🖼️', 'bmp': '🖼️', 'webp': '🖼️', 'svg': '🖼️',
        'exe': '⚙️', 'msi': '⚙️', 'dll': '⚙️',
        'js': '💻', 'ts': '💻', 'py': '💻', 'java': '💻', 'cs': '💻', 'cpp': '💻', 'c': '💻', 'html': '💻', 'css': '💻',
        'iso': '💿',
    };
    return icons[ext] || '📦';
}

function scrollToBottom() {
    const container = document.getElementById('chatMessages');
    setTimeout(() => {
        container.scrollTop = container.scrollHeight;
    }, 50);
}

// ====== Auto-resize textarea ======
function initTextareaResize() {
    const textarea = document.getElementById('messageInput');
    textarea.addEventListener('input', () => {
        textarea.style.height = 'auto';
        textarea.style.height = Math.min(textarea.scrollHeight, 120) + 'px';
    });
}

// ====== Keyboard shortcuts ======
function initKeyboard() {
    const textarea = document.getElementById('messageInput');
    textarea.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });

    // Escape to close lightbox / emoji picker
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeLightbox();
            document.getElementById('emojiPicker').style.display = 'none';
        }
    });
}

// ====== Init ======
document.addEventListener('DOMContentLoaded', () => {
    initSignalR();
    initEmojiPicker();
    initFileAttachment();
    initTextareaResize();
    initKeyboard();

    document.getElementById('sendBtn').addEventListener('click', sendMessage);
});
